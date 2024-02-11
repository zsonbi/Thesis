<?php

// Include config file
require_once "config.php";
define('DEFAULT_PERIOD', '1440');

class TasksModel
{
    private $link;
    private $tableName;

    //************************************************************************************** */
    /**
     * Creates the link to the database
     */
    function __construct(int $userId)
    {
        $this->link = mysqli_connect(DB_SERVER, DB_USERNAME, DB_PASSWORD, DB_NAME);
        // Check connection
        if ($this->link->connect_error) {
            die("Connection failed: " . $this->link->connect_error);
        }

        $this->tableName = TASKS_TABLE . $userId;
    }

    //********************************************************************************************* */
    /**
     * Closes the link
     */
    public function __destruct()
    {
        if ($this->link != null)
            $this->link->close();
    }


    public function saveTask(string $taskName, int $taskType, string $description, int $periodRate = DEFAULT_PERIOD, int $id=-1)
    {
        if($id ==-1){
        $stmt = $this->link->prepare("INSERT INTO `" . $this->tableName . "`(`taskName`, `taskType`, `periodRate`,`description`) VALUES (?,?,?,?)");
        $stmt->bind_param("siis", $taskName, $taskType, $periodRate, $description);

        }
        else{

            $stmt = $this->link->prepare("UPDATE " . $this->tableName . " SET `taskName`=? ,`taskType`=?, `periodRate`=?, `description`=?  WHERE id=?");
            $stmt->bind_param("siisi", $taskName, $taskType, $periodRate, $description,$id);
        }


        $stmt->execute();
        $taskId = $id == -1 ? $stmt->insert_id : $id;
        $stmt->store_result();
        if ($stmt->affected_rows == 0) {
            $stmt->close();
            return null;
        }
        $stmt->close();
        return $this->getTaskById($taskId);
    }

    public function getTasks()
    {
        $stmt = $this->link->prepare("SELECT * FROM " . $this->tableName);

        if ($stmt->execute() ) {
            $result = $stmt->get_result()->fetch_all(MYSQLI_ASSOC);
        }

        $stmt->close();
        return $result;
    }

    public function getTasksByType(int $taskType)
    {

        $stmt = $this->link->prepare("SELECT * FROM " . $this->tableName . " WHERE taskType=?");
        $stmt->bind_param("i", $taskType);

        if ($stmt->execute() ) {
            $result = $stmt->get_result()->fetch_all(MYSQLI_ASSOC);
        }

        $stmt->close();

        return $result;
    }


    public function getTaskById(int $id)
    {
        $stmt = $this->link->prepare("SELECT * FROM " . $this->tableName . " WHERE id=?");
        $stmt->bind_param("i", $id);
        if ($stmt->execute()) {
            $result = $stmt->get_result();
        }

        $stmt->close();
        return $result->fetch_array(MYSQLI_ASSOC);
    }


    public function completeTask(int $id)
    {
        $stmt = $this->link->prepare("SELECT (Unix_TimeSTAMP(CURRENT_TIMESTAMP())-UNIX_TIMESTAMP(lastCompleted))/60 as elapsedMinutes, periodRate FROM " . $this->tableName . " WHERE id = ?");
        $stmt->bind_param("i", $id);
        if ($stmt->execute()) {
            $result = $stmt->get_result()->fetch_array(MYSQLI_ASSOC);

            if ($result["elapsedMinutes"] >= $result["periodRate"]) {
                $stmt = $this->link->prepare("UPDATE " . $this->tableName . " SET `lastCompleted`=CURRENT_TIMESTAMP(),`completed`='1' WHERE id=?");
                $stmt->bind_param("i", $id);
                if ($stmt->execute()) {
                    $stmt->close();
                    return true;
                }
            }
        }
        $stmt->close();

        return false;
    }

    public function deleteTask(int $id){
     
                $stmt = $this->link->prepare("DELETE FROM " . $this->tableName . " WHERE id=?");
                $stmt->bind_param("i", $id);
                if ($stmt->execute() && $stmt->affected_rows != 0) {
                    $stmt->close();
                    return true;
                }

        return false;


    }
}
