<?php
require_once('./Models/userModel.php');



 
    // Check if username is empty
    if(empty(trim($_POST["Username"]))){
        $username_err = "Please enter username.";
    } else{
        $username = trim($_POST["Username"]);
    }
    
    // Check if password is empty
    if(empty(trim($_POST["Password"]))){
        $password_err = "Please enter your password.";
    } else{
        $password = trim($_POST["Password"]);
    }

    // Check if email is empty
    if(empty(trim($_POST["Email"]))){
        $email_err = "Please enter your email.";
    } else{
        $email = trim($_POST["Email"]);
    }
   

    if(empty($username_err) && empty($password_err) && empty($email_err)){ 

        $userModel = new UserModel();
       
       if( $userModel->addNewUser($email,$username,$password))
        {
            $user = $userModel->getUserByUserName($username,true);
            $resArray=array_merge(array("res"=>"success"),$user);
            echo json_encode($resArray);
        }
        else{
            $err=array();
            
            array_merge($err,array("error"=> "Already exist such account with this email or username"));

            echo json_encode(array_merge($err, array("res"=>"Failed to create account")));
            echo "Failed to create user";
        }
    }
    else{
        $err=array();
        if(!empty($username_err)){
           $err= array_merge($err,array("error"=> $username_err));
        }
        else if(!empty($password_err)){
          $err=  array_merge($err,array("error"=> $password_err));
        }
        else if(!empty($email_err)){
           $err= array_merge($err,array("error"=> $email_err));
        }

        echo json_encode(array_merge($err, array("res"=>"Failed to create account")));

    }




?>