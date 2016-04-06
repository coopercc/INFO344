<!DOCTYPE html>
<html>
<head>
	<title></title>
</head>
<body>
	<form action="getEvenNumbers.php" method="post">
		Pick a Number: <input type="text" name="n"></input>

		<input type="submit">
	</form>

	<p>
		<?php

		$n = $_POST["n"];
		for ($i = 2; $i <= $n; $i++) {
			echo $i;
			$i = $i + 2;
		}
		?>
	</p>
</body>
</html>