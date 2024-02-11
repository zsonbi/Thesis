<?php
ini_set('display_errors', 1);
ini_set('display_startup_errors', 1);
error_reporting(E_ALL);


define('DB_SERVER', 'localhost');
define('DB_USERNAME', 'thesisUser');
define('DB_PASSWORD', 'ETpUxDdXwt9s7RXVhc8b9PUkR2Ttt8xPVqn');
define('DB_NAME', 'Thesis');
define('TASKS_TABLE',"tasks_table");

$link = mysqli_connect(DB_SERVER, DB_USERNAME, DB_PASSWORD, DB_NAME);
 
// Check connection
if($link === false){
    die("ERROR: Could not connect. " . mysqli_connect_error());
}

// Initialize the session
session_start();

?>