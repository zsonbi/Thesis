<?php
require_once('./Models/tasksModel.php');

if(!(isset($_SESSION["loggedin"]) && $_SESSION["loggedin"] === true)){
   return;
}


 
    // Check if task name is empty
    if(empty(trim($_POST["taskName"]))){
        $taskName_err = "Please enter your task's name.";
    } else{
        $taskName = trim($_POST["taskName"]);
    }
    
    // Check if description is set
    if(!empty($_POST["description"]))
    {   
        $description = trim($_POST["description"]);
    }
    else{
        $description="";
    }

    // Check if periodRate is empty
    if(empty(trim($_POST["periodRate"]))){
        $periodRate_err = "Please enter your period rate.";
    } else{
        $periodRate = trim($_POST["periodRate"]);
    }

    //Check if taskType is set
       if(!isset($_POST["taskType"])){
        $taskType_err = "Please enter your taskType.";
    } else{
        $taskType = trim($_POST["taskType"]);
    }
   

    if(empty($periodRate_err) && empty($taskType_err) && empty($taskName_err)){ 

        $tasksModel = new TasksModel($_SESSION["id"]);
       
        $id = $_POST["id"] ?? -1;

        $task = $tasksModel->saveTask($taskName,$taskType,$description,$periodRate,$id);

        if(is_array($task))
        {
            
            $resArray=array_merge(array("res"=>"success"),$task);
            echo json_encode($resArray);
        }
        else{

            echo json_encode(array("res"=> "Failed to add the task"));

        }
    }
    else{
        $err=array();
        if(!empty($periodRate_err)){
           $err= array_merge($err,array("error"=> $periodRate_err));
        }
        else if(!empty($taskType_err)){
          $err=  array_merge($err,array("error"=> $taskType_err));
        }
        else if(!empty($taskName_err)){
            $err= array_merge($err,array("error"=> $taskName_err));
        }

        echo json_encode(array_merge($err, array("res"=>"Failed to add the task")));
    }
