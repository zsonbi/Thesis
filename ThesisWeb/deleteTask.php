<?php
require_once('./Models/tasksModel.php');

if(!(isset($_SESSION["loggedin"]) && $_SESSION["loggedin"] === true)){
   return;
}

   if( empty($_POST["id"])){
    echo json_encode(array("res"=> "Can't complete the task no task were given"));
    
    return;
   }
 

        $tasksModel = new TasksModel($_SESSION["id"]);
       
        $deleted = $tasksModel->deleteTasK($_POST["id"]);

       if($deleted)
        {
            echo json_encode(array("res"=>"success"));
        }
        else{
            echo json_encode(array("res"=> "Can't delete the task", "error"=>"No such task exists!"));
        }
    
   ?>