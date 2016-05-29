<?php 
require('player.php');
$searchName = $_GET["name"];
$callback = $_GET["callback"];

$returnData = array();	//sets the array of all players who match the search name

try {
	$conn = new PDO('mysql:host=pa1.crm76mq5rzuz.us-west-2.rds.amazonaws.com;port=3306;dbname=PA1', 'info344user', 'Homerun240');
    $conn->setAttribute(PDO::ATTR_ERRMODE, PDO::ERRMODE_EXCEPTION);

	$stmt = $conn->prepare("SELECT * FROM Stats WHERE Name = '{$searchName}' LIMIT 1");
	$stmt->execute();
	$data = $stmt->fetchAll();

    foreach($data as $row) {
    	$returnData = array(name => $row['Name'], team => $row['Team'], gp => $row['GP'], threept => $row['3PT-M'], offReb => $row['Rebounds-Off'], defReb=> $row['Rebounds-Def'], totReb => $row['Rebounds-Tot'], ast => $row['Ast'], turnovers =>$row['TO'], stl => $row['Stl'], block => $row['Blk'], ppg => $row['PPG']);

    echo $callback . '(' . json_encode($returnData) . ')';
    }
    /*
    $returnData = array(name => $data['Name'], team => $data['Team'], gp => $data['GP'], threept => $data['3PT-M'], offReb => $data['Rebounds-Off'], defReb=> $data['Rebounds-Def'], totReb => $data['Rebounds-Tot'], ast => $data['Ast'], turnovers =>$data['TO'], stl => $data['Stl'], block => $data['Blk'], ppg => $data['PPG']);

    echo $callback . '(' . json_encode($returnData) . ')';
    */

} catch(PDOException $e) {
    echo 'ERROR: ' . $e->getMessage();
}
?>
