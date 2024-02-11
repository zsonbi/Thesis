<?php
require_once('./Models/tasksModel.php');

if(!(isset($_SESSION["loggedin"]) && $_SESSION["loggedin"] === true)){
   return;
}

   
 

        $tasksModel = new TasksModel($_SESSION["id"]);
       
        $tasks = $tasksModel->getTasks();

       if(is_array($tasks))
        {
            
            $resArray=array_merge(array("res"=>"success"),array("tasks"=> $tasks));
            echo json_encode($resArray);
        }
        else{


            

            echo json_encode(array("res"=> "Failed to get the tasks"));

        }
    
   ?>