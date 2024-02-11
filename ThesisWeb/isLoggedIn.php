<?php

require_once('./Models/userModel.php');


 
// Check if the user is already logged in, if yes then redirect him to welcome page
if(isset($_SESSION["loggedin"]) && $_SESSION["loggedin"] === true){
    $userModel = new UserModel();
    $user = $userModel->getUserById( $_SESSION["id"], true);
    $resArray=array_merge(array("res"=>"success"),$user);
    echo json_encode($resArray);
}



?>