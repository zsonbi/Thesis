<?php

// Include config file
require_once "config.php";
define('DEFAULT_PRIVACY', '255');

class UserModel{
    private $link;

    //************************************************************************************** */
    /**
     * Creates the link to the database
     */
    function __construct()
    {
        $this->link = mysqli_connect(DB_SERVER, DB_USERNAME, DB_PASSWORD, DB_NAME);
        // Check connection
        if ($this->link->connect_error) {
            die("Connection failed: " . $this->link->connect_error);
        }
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


    public function addNewUser(string $email, string $userName, string $password){
        if($this->getUserByEmail($email) != null || $this->getUserByUserName($userName) ){
            return false;
        }
        
        $stmt = $this->link->prepare("INSERT INTO `users`(`username`, `password`, `email`,`settingsId`) VALUES (?,?,?,?)");
        $hashedPass=password_hash($password, PASSWORD_DEFAULT);
        $settingsId = $this->addUserSettings();
        $stmt->bind_param("sssi", $userName,$hashedPass,$email,$settingsId);
        $stmt->execute();
        $userId = $stmt->insert_id;
        $stmt->store_result();
        if ($stmt->affected_rows == 0) {
            $stmt->close();
            return false;
        }
        $tableName =TASKS_TABLE.$userId;
        $stmt=$this->link->prepare("CREATE TABLE ".$tableName." ( id INT PRIMARY KEY AUTO_INCREMENT, taskName TEXT, taskType TINYINT, periodRate INT, description TEXT, added TIMESTAMP default CURRENT_TIMESTAMP, lastCompleted TIMESTAMP default \"1970-01-01 11:11:11\", completed TINYINT DEFAULT 0)");
        $stmt->execute();
        // Store data in session variables
        $_SESSION["loggedin"] = true;
        $_SESSION["id"] = $userId;
        $_SESSION["username"] = $userName;    

        $stmt->close();
        return true;
    }

    public function validateUser(string $userIdentification, string $password, bool $updateLoggedInTime=false){
        $user = $this->getUserByIdentification($userIdentification);
       
       

        if(password_verify($password, $user["password"])){
                                
        // Store data in session variables
        $_SESSION["loggedin"] = true;
        $_SESSION["id"] = $user["id"];
        $_SESSION["username"] = $user["username"];       
        return true;
        }
        else {
            return false;
        }

    }

    public function getUserById(int $id, bool $removePassword=false){
        $stmt = $this->link->prepare("SELECT * FROM `users` WHERE id=?");
        $stmt->bind_param("i", $id);
        if($stmt->execute() == true)
        {
           $result = $stmt->get_result();
           while($row = $result->fetch_array(MYSQLI_ASSOC))
           {
            if($removePassword){
                unset($row["password"]);
            }
          return $row;
           }
        }
        $stmt->close();
        return null;
    }

    public function getUserByEmail(string $email, bool $removePassword=false){



        $stmt = $this->link->prepare("SELECT * FROM `users` WHERE email=?");
        $stmt->bind_param("s", $email);
        if($stmt->execute() == true)
        {
           $result = $stmt->get_result();
           while($row = $result->fetch_array(MYSQLI_ASSOC))
           {
                if($removePassword){
                    unset($row["password"]);
                }
              return $row;
           }
        }
        $stmt->close();
        return null;
    }

    public function getUserByUserName(string $userName, bool $removePassword=false) {
       
        

        $stmt = $this->link->prepare("SELECT * FROM `users` WHERE username=?");
        $stmt->bind_param("s", $userName);
        if($stmt->execute() == true)
        {
           $result = $stmt->get_result();
           while($row = $result->fetch_array( MYSQLI_ASSOC))
           {
            if($removePassword){
                unset($row["password"]);
            }
  

              return $row;
           }
        }
        $stmt->close();
        return null;
    }

    public function getUserByIdentification(string $userIdentification, bool $removePassword=false){

        $user = $this->getUserByEmail($userIdentification,$removePassword);
        if($user == null){
            $user = $this->getUserByUserName($userIdentification,$removePassword);
            if($user == null){
                return null;
            }
        }
        

        return $user;


    }

    private function addUserSettings(){
        $stmt = $this->link->prepare("INSERT INTO `userSettings`(`privacy`) VALUES (?)");

        $privacy = DEFAULT_PRIVACY;
      
        $stmt->bind_param("i", $privacy);
        $stmt->execute();
        $settingsId = $stmt->insert_id;
        $stmt->store_result();
        if ($stmt->affected_rows == 1) {
            $stmt->close();
            return $settingsId;
        }
    
        $stmt->close();
        return -1;


    }

}
