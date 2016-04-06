
<?php 

echo $_GET["name"];


try {
	$conn = new PDO('mysql:host=pa1.crm76mq5rzuz.us-west-2.rds.amazonaws.com;port=3306;dbname=PA1', 'info344user', 'Homerun240');
    $conn->setAttribute(PDO::ATTR_ERRMODE, PDO::ERRMODE_EXCEPTION);

    $searchName = $_GET["name"];


	$stmt = $conn->prepare('SELECT * FROM Stats');
		$stmt->execute();


    while($row = $stmt->fetch()) {
        print_r($row);
    }

/*
	THIS IS HOW I GOT JUST BOOK NAMES ON IN CLASS ASSN
    $data = $conn->prepare('SELECT * FROM Books');
    $data->execute();
 


    foreach($data as $row) {
        $name = $row['name'];
        print_r($name . " ");
        echo  "\n";

    }
*/

} catch(PDOException $e) {
    echo 'ERROR: ' . $e->getMessage();
}

 ?>