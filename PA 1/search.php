<?php 
require('player.php');
$searchName = $_GET["name"];

$allPlayers;	//sets the array of all players who match the search name

try {
	$conn = new PDO('mysql:host=pa1.crm76mq5rzuz.us-west-2.rds.amazonaws.com;port=3306;dbname=PA1', 'info344user', 'Homerun240');
    $conn->setAttribute(PDO::ATTR_ERRMODE, PDO::ERRMODE_EXCEPTION);

	$stmt = $conn->prepare("SELECT * FROM Stats WHERE Name LIKE '%$searchName%'");
	$stmt->execute();
	$data = $stmt->fetchAll();

	//creates a player for all the people who match the search name. Also sets the array of all players
    foreach ($data as $row) {
    	$newPlayer = new Player($row['Name'], $row['Team'], $row['GP'], $row['3PT-M'], $row['Rebounds-Off'], $row['Rebounds-Def'], $row['Rebounds-Tot'], $row['Ast'], $row['TO'], $row['Stl'], $row['Blk'], $row['PPG']);

    	$allPlayers = $newPlayer->getAllPlayers();
    }

} catch(PDOException $e) {
    echo 'ERROR: ' . $e->getMessage();
}
?>
<!DOCTYPE html>
<html>
<head>
	<title>Search Results</title>
	<link rel="stylesheet" href="http://maxcdn.bootstrapcdn.com/bootstrap/3.3.6/css/bootstrap.min.css">
	<link rel="stylesheet" type="text/css" href="css/main.css">
	<link rel="icon" href="img/tab-icon.png">
</head>
<body>
	<!-- Form to search again -->
	<div class="row">
		<img id="logo" src="img/nba.jpg">

		<form action="search.php" method="get">
		Would you like to search again?
		<fieldset class="form-group">
			Player Name: <input type="text" name="name">
		</fieldset>
		<input type="submit">
	</form>
	</div>

<?php
	//displays all players that match the search term
	foreach ($allPlayers as $player) {
		echo "<div class='well'>";
			echo "<h1>" . $player->getName() . "</h1>";
			echo "<table>";
				echo "<tr>";
					echo "<th>Team</th>";
					echo "<th>Games Played</th>";
					echo "<th>3PT Made</th>";
					echo "<th>Offensive Rebounds</th>";
					echo "<th>Defensive Rebounds</th>";
					echo "<th>Total Rebounds</th>";
					echo "<th>Assists</th>";
					echo "<th>Turnovers</th>";
					echo "<th>Steals</th>";
					echo "<th>Blocks</th>";
					echo "<th>PPG</th>";
				echo "</tr>";

				echo "<tr>";
					echo "<td>" . $player->getTeam() . "</td>";
					echo "<td>" . $player->getGP() . "</td>";
					echo "<td>" . $player->getThrees() . "</td>";
					echo "<td>" . $player->getOffReb() . "</td>";
					echo "<td>" . $player->getDefReb() . "</td>";
					echo "<td>" . $player->getTotReb() . "</td>";
					echo "<td>" . $player->getAssists() . "</td>";
					echo "<td>" . $player->getTO() . "</td>";
					echo "<td>" . $player->getStl() . "</td>";
					echo "<td>" . $player->getBlocks() . "</td>";
					echo "<td>" . $player->getPpg() . "</td>";
				echo "</tr>";
			echo "</table>";
		echo "</div>";
	}
?>
</body>
</html>