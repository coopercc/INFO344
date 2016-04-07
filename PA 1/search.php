<?php 
require('player.php');
$searchName = $_GET["name"];

try {
	$conn = new PDO('mysql:host=pa1.crm76mq5rzuz.us-west-2.rds.amazonaws.com;port=3306;dbname=PA1', 'info344user', 'Homerun240');
    $conn->setAttribute(PDO::ATTR_ERRMODE, PDO::ERRMODE_EXCEPTION);

	$stmt = $conn->prepare("SELECT * FROM Stats WHERE Name LIKE '%$searchName%'");
	$stmt->execute();
	$data = $stmt->fetchAll();


    foreach ($data as $row) {
    	$newPlayer = new Player($row['Name'], $row['Team'], $row['GP'], $row['3PT-M'], $row['Rebounds-Off'], $row['Rebounds-Def'], $row['Rebounds-Tot'], $row['Ast'], $row['TO'], $row['Stl'], $row['Blk'], $row['PPG']);
    }

} catch(PDOException $e) {
    echo 'ERROR: ' . $e->getMessage();
}
?>
<!DOCTYPE html>
<html>
<head>
	<title>Search Results</title>
</head>
<body>
	<!-- Form to search again -->
	<div>
		Would you like to search again?
		<form action="search.php" method="get">
		Player Name: <input type="text" name="name">
		<input type="submit">
	</form>
	</div>

	<!-- Display the results  -->
	<div>
		
	</div>
</body>
</html>