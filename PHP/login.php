<?php
    // Database =========================================================
    $host = "localhost";
    $user = "yourUsername";
    $password = "yourPassword";
    $dbname = "yourDBname";
    
    $mysqli = new mysqli($host, $user, $password, $dbname) or die("Can't connect to database");
    // =============================================================================

    $Action = $_POST["Action"]; // which action, Login or Register?
    $nick = $_POST["User"];
    $pass = $_POST["Pass"];

    if($Action == "Login"){
        if(empty($nick) || empty($pass)) {
            echo "Username or password can't be empty.";
        } else {
                $SQL = "SELECT * FROM users WHERE username = '" . $nick . "'";
                $result_id = $mysqli->query($SQL) or die("DB ERROR");
                $total = mysqli_num_rows($result_id);
                    if($total) {
                        $data = @mysqli_fetch_array($result_id);
                            if(strcmp($pass, $data["password"]) == 0) {
                                echo "Logged in!";
                            } else {
                                echo "You entered a wrong username or password.";
                            }
                    } else {
                        echo "No account with that username was found.";
                }
        }
    }

   if($Action == "Register"){
        $Email = $_POST["Email"]; 

        $checkuser = $mysqli->query("SELECT username FROM users WHERE username='$nick'"); 
        $username_exist = mysqli_num_rows($checkuser);
        if($username_exist > 0) {
              echo "Username already taken.";// Username is taken
              unset($nick);
              die();
        } else {
            $query = "INSERT INTO users (username, password, email) VALUES('$nick', '$pass', '$Email')";
            $mysqli->query($query) or die("Error during registration :(");
            mysqli_close($mysqli);
            echo "Registered!";
        }
    }
    ?>