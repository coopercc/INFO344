<?php 

$allPlayers = array();

class Player {
	private $name;
	private $team;
	private $GP;
	private $threePtM;
	private $reboundsOff;
	private $reboundsDef;
	private $reboundsTot;
	private $assists;
	private $TO;
	private $stl;
	private $block;
	private $ppg;

	function __construct($name, $team, $GP, $threePtM, $reboundsOff, $reboundsDef, $reboundsTot, $assists, $TO, $stl, $block, $ppg) {

		$this->name = $name;
		$this->team = $team;
		$this->GP = $GP;
		$this->threePtM = $threePtM;
		$this->reboundsOff = $reboundsOff;
		$this->reboundsDef = $reboundsDef;
		$this->reboundsTot = $reboundsTot;
		$this->assists = $assists;
		$this->TO = $TO;
		$this->stl = $stl;
		$this->block = $block;
		$this->ppg = $ppg;

		global $allPlayers;
		array_push($allPlayers, $this);
	}

	function getName() {
		return $this->name;
	}

	function getTeam() {
		return $this->team;
	}

	function getGP() {
		return $this->GP;
	}

	function getThrees() {
		return $this->threePtM;
	}

	function getOffReb() {
		return $this->reboundsOff;
	}

	function getDefReb() {
		return $this->reboundsDef;
	}

	function getTotReb() {
		return $this->reboundsTot;
	}

	function getAssists() {
		return $this->assists;
	}

	function getTO() {
		return $this->TO;
	}

	function getStl() {
		return $this->stl;
	}

	function getBlocks() {
		return $this->block;
	}

	function getPpg() {
		return $this->ppg;
	}

	function getAllPlayers() {
		return $allPlayers;
	}

}


?>