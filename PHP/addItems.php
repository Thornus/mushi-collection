<?php
    function databaseAccess() {
        $host = "localhost";
        $user = "yourUsername";
        $password = "yourPassword";
        $dbname = "yourDBname";
    
        
        $GLOBALS["mysqli"] = new mysqli($host, $user, $password, $dbname) or die("Can't connect to database");
    }
    
    function getUserId($username) {
        $result = $GLOBALS["mysqli"]->query("SELECT id FROM users WHERE username = '".$username."'") or die("DATABASE ERROR: can't get username");
        $row = mysqli_fetch_assoc($result);
        return $row["id"];
    }

    function checkIfItemExists($itemName) {
        $sql = "SELECT name FROM items JOIN users ON (users.id = items.userId) WHERE name = '".$itemName."' AND userId = ".$GLOBALS["userId"];
        $stmt =$GLOBALS["mysqli"]->prepare($sql) or die("DATABASE ERROR: ".$stmt->error);
        $stmt->execute() or die("DATABASE ERROR: couldn't check item amount ||| ".$stmt->error);
        $result = $stmt->get_result();

        if(mysqli_num_rows($result) > 0) {
            return true;
        } else {
            return false;
        }
    }

    function createInsertAndVaryAmountQuery() { //will return false if insert query shouldn't be executed
        global $insertSql, $bindParamString, $numberOfItemsToAdd, $itemsToAddDataArray, $varyAmountSql;
        $insertSql = "INSERT INTO items (id, name, description, typeId, amount, userId) VALUES ";
        
        $bindParamString = "";
        $numberOfItemsToAddTemp = $numberOfItemsToAdd;
        $n = 0; //counts items that will be added with insert query
        for($i = 0; $i < $numberOfItemsToAdd; $i++) {
            if(checkIfItemExists($_POST["itemName".$i]) == false) {
                if($i < $numberOfItemsToAdd - 1)
                    $insertSql .= "(NULL, ?, ?, ?, ?, ?), ";
                else
                    $insertSql .= "(NULL, ?, ?, ?, ?, ?)";
                
                $bindParamString .= "sssii";
                
                $itemsToAddDataArray["itemName".$n] = $_POST["itemName".$i];
                $itemsToAddDataArray["itemDescription".$n] = $_POST["itemDescription".$i];
                $itemsToAddDataArray["typeId".$n] = $_POST["typeId".$i];
                $itemsToAddDataArray["itemAmount".$n] = $_POST["itemAmount".$i];
                $n++;
            } else {
                $numberOfItemsToAddTemp--; //decrease number of items to add because some already exist in the db
            }
        }
        
        if($insertSql == "INSERT INTO items (id, name, description, typeId, amount, userId) VALUES (NULL, ?, ?, ?, ?, ?), ") {
            $insertSql = "INSERT INTO items (id, name, description, typeId, amount, userId) VALUES (NULL, ?, ?, ?, ?, ?)"; //if only one item is to be added, the comma must be removed from the query
        }
        
        echo $insertSql;
        
        $varyAmountSql = "UPDATE items JOIN users ON (users.id = items.userId) SET amount = CASE name";
        $appendToEndOfVaryAmountSql = "WHERE name IN (";
        for($i = 0; $i < $numberOfItemsToAdd; $i++) {
            $varyAmountSql .= " WHEN '".$_POST["itemName".$i]. "' THEN ".$_POST["itemAmount".$i];
            
            if($i < $numberOfItemsToAdd - 1) {
                $appendToEndOfVaryAmountSql .= "'".$_POST["itemName".$i]."' ,";
            } else {
                $appendToEndOfVaryAmountSql .= "'".$_POST["itemName".$i]."') AND users.id = ".$GLOBALS["userId"];
                $varyAmountSql .= " END ".$appendToEndOfVaryAmountSql;
            }

        }
        
        $numberOfItemsToAdd = $numberOfItemsToAddTemp;
        echo $varyAmountSql;
        
        if($insertSql == "INSERT INTO items (id, name, description, typeId, amount, userId) VALUES ") { //no item has to be added
            echo "Only items amount will vary";
            return false;
        } else {
            return true;
        }
    }

    $mysqli;
    $insertSql;
    $varyAmountSql;
    $itemsToAddDataArray; //contains only the data of the items that will actually be added with an insert
    databaseAccess(); //connect to db

    //Get data posted by game and add item to db =============
    $userId = getUserId($_POST["username"]);

    $numberOfItemsToAdd = $_POST["numberOfItemsToAdd"];

    if(createInsertAndVaryAmountQuery()) {
        $stmt = $mysqli->prepare($insertSql) or die("DATABASE ERROR: ".$stmt->error); //statement prepare

        $a_params = array();
        $a_params[] = & $bindParamString;
        for($i = 0; $i < $numberOfItemsToAdd; $i++) {
            $a_params[] = & $itemsToAddDataArray["itemName".$i];
            $a_params[] = & $itemsToAddDataArray["itemDescription".$i];
            $a_params[] = & $itemsToAddDataArray["typeId".$i];
            $a_params[] = & $itemsToAddDataArray["itemAmount".$i];
            $a_params[] = & $userId;
        }
    
        echo $numberOfItemsToAdd;
        call_user_func_array(array($stmt, 'bind_param'), $a_params);

        if ($stmt->execute()) { //if successful
            echo "Item(s) added successfully!";
        } else {
            echo "DATABASE ERROR: couldn't add item ||| ".$stmt->error;
        }
        
        $stmt = $mysqli->prepare($varyAmountSql) or die("DATABASE ERROR: ".$stmt->error); //statement prepare
        if ($stmt->execute()) { //if successful
            echo "Item(s) amount modified!";
        } else {
            echo "DATABASE ERROR: couldn't modify item(s) amount ||| ".$stmt->error;
        }
    } else {
        $stmt = $mysqli->prepare($varyAmountSql) or die("DATABASE ERROR: ".$stmt->error); //statement prepare
        if ($stmt->execute()) { //if successful
            echo "Item(s) amount modified!";
        } else {
            echo "DATABASE ERROR: couldn't modify item(s) amount ||| ".$stmt->error;
        }
    }

    $mysqli->close();
?>