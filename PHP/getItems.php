<?php
    //Database access ========================================
    $host = "localhost";
    $user = "yourUsername";
    $password = "yourPassword";
    $dbname = "yourDBname";
    
    $mysqli = new mysqli($host, $user, $password, $dbname) or die("Can't connect to database");

    $result = $mysqli->query("SELECT id FROM users WHERE username = '".$_POST["username"]."'") or die("DATABASE ERROR: can't get username");
    $row = mysqli_fetch_assoc($result);
    $userId = $row["id"];

    $sql = "SELECT items.id, name, typeId, description, amount FROM items JOIN users ON (items.userId = users.id) JOIN itemTypes ON (items.typeId = itemTypes.id) WHERE items.userId = ".$userId;

    $stmt = $mysqli->prepare($sql) or die("DATABASE ERROR: ".$stmt->error); //statement prepare
    $stmt->execute() or die("DATABASE ERROR: couldn't get items");
    $result = $stmt->get_result();
    if(mysqli_num_rows($result) > 0) {
        while ($row = mysqli_fetch_assoc($result))  {
            echo $row["id"].";".$row["name"].";".$row["description"].";".$row["typeId"].";".$row["amount"]."|";
        }
    } else {
        echo "none"; //result is empty
    }
        
    $mysqli->close();
    ?>