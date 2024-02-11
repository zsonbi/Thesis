<?php
require_once('./Models/userModel.php');

    // Check if username is empty
    if(empty(trim($_POST["UserIdentification"]))){
        $userIdentification_err = "Please enter username.";
    } else{
        $userIdentification = trim($_POST["UserIdentification"]);
    }
    
    // Check if password is empty
    if(empty(trim($_POST["Password"]))){
        $password_err = "Please enter your password.";
    } else{
        $password = trim($_POST["Password"]);
    }

    if(empty($username_err) && empty($password_err)){ 

        $userModel = new UserModel();
        if( $userModel->validateUser($userIdentification,$password,true))
        {
            $user = $userModel->getUserByIdentification($userIdentification,true);
            $resArray=array_merge(array("res"=>"success"),$user);
            echo json_encode($resArray);
            
        }
        else{
            echo json_encode(array("res"=>"Failed to log in"));
        }       

    }
    else{
        $err=array();
        if(!empty($username_err)){
           $err= array_merge($err,array("error"=> $username_err));
        }
        if(!empty($password_err)){
            $err= array_merge($err,array("error"=> $password_err));
        }

        echo json_encode(array_merge($err, array("res"=>"Failed to log in")));


    }


?>