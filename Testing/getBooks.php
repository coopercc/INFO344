<?php 
try {
    $conn = new PDO('mysql:host=uwinfo344.chunkaiw.com;dbname=info344mysqlpdo', 'info344mysqlpdo', 'chrispaul');
    $conn->setAttribute(PDO::ATTR_ERRMODE, PDO::ERRMODE_EXCEPTION);    
     
    $stmt = $conn->prepare('SELECT * FROM Books');
    $stmt->execute();
 
    while($row = $stmt->fetch()) {
        print_r($row);
    }
} catch(PDOException $e) {
    echo 'ERROR: ' . $e->getMessage();
}
?>