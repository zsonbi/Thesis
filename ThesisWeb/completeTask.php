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
       
        $completed = $tasksModel->completeTask($_POST["id"]);

       if($completed)
        {
            
          
            echo json_encode(array("res"=>"success"));
        }
        else{
            echo json_encode(array("res"=> "Can't complete the task"));

        }
    
   ?>