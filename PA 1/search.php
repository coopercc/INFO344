Hello World
<?php 

	echo $_GET["name"];


	try {
		$conn = new PDO('mysql:host=pa1.crm76mq5rzuz.us-west-2.rds.amazonaws.com;port=3306;dbname=PA1', 'info344user', 'Homerun240');
	    $conn->setAttribute(PDO::ATTR_ERRMODE, PDO::ERRMODE_EXCEPTION);

	    $searchName = $_GET["name"];
	    /*

		$stmt = $conn->prepare('SELECT * FROM Stats WHERE Name = :searchName');
   		$stmt->execute(array('Name' => $searchName));
 
	    while($row = $stmt->fetch()) {
	        print_r($row);
	    }

	}

	} catch(PDOException $e) {
	    echo 'ERROR: ' . $e->getMessage();
	}
		    */
 ?>