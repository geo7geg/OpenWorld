-- phpMyAdmin SQL Dump
-- version 4.2.7.1
-- http://www.phpmyadmin.net
--
-- Φιλοξενητής: 127.0.0.1
-- Χρόνος δημιουργίας: 05 Αυγ 2015 στις 11:37:53
-- Έκδοση διακομιστή: 5.6.20
-- Έκδοση PHP: 5.5.15

SET SQL_MODE = "NO_AUTO_VALUE_ON_ZERO";
SET time_zone = "+00:00";


/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!40101 SET NAMES utf8 */;

--
-- Βάση δεδομένων: `mapgame2`
--
CREATE DATABASE IF NOT EXISTS `mapgame2` DEFAULT CHARACTER SET latin1 COLLATE latin1_swedish_ci;
USE `mapgame2`;

-- --------------------------------------------------------

--
-- Δομή πίνακα για τον πίνακα `agreement`
--

CREATE TABLE IF NOT EXISTS `agreement` (
  `id` int(30) NOT NULL,
  `player_a` varchar(30) NOT NULL,
  `player_b` varchar(30) NOT NULL,
  `region_a` varchar(30) NOT NULL,
  `region_b` varchar(30) NOT NULL,
  `player_c` varchar(30) NOT NULL,
  `sunaspismos` varchar(30) NOT NULL,
  `enwsi` varchar(30) NOT NULL,
  `leader` varchar(30) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=latin1;

-- --------------------------------------------------------

--
-- Δομή πίνακα για τον πίνακα `attacks`
--

CREATE TABLE IF NOT EXISTS `attacks` (
  `id` int(30) NOT NULL,
  `att_player` varchar(30) NOT NULL,
  `def_player` varchar(30) NOT NULL,
  `att_region` varchar(50) NOT NULL,
  `def_region` varchar(50) NOT NULL,
  `distance` int(20) NOT NULL,
  `turn` int(20) NOT NULL,
  `army` int(20) NOT NULL,
  `kind` int(20) NOT NULL,
  `army_sunasp` int(30) NOT NULL,
  `sunaspismos` varchar(30) NOT NULL,
  `army_enwsi` int(30) NOT NULL,
  `enwsi` varchar(30) NOT NULL,
  `army_coplayer` varchar(300) NOT NULL,
  `region_coplayer` varchar(300) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=latin1;

--
-- Δείκτες `attacks`
--
DELIMITER //
CREATE TRIGGER `attack_events` AFTER INSERT ON `attacks`
 FOR EACH ROW INSERT INTO events (att_player,def_player,att_region,def_region,army,kind,inserton,date) VALUES (NEW.att_player,NEW.def_player,NEW.att_region,NEW.def_region,NEW.army,NEW.kind,NOW(),NOW())
//
DELIMITER ;

-- --------------------------------------------------------

--
-- Δομή πίνακα για τον πίνακα `def_othomanoi`
--

CREATE TABLE IF NOT EXISTS `def_othomanoi` (
  `id` int(10) NOT NULL,
  `name` varchar(200) NOT NULL,
  `factor` int(10) NOT NULL,
  `flag` int(5) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=latin1;

--
-- Άδειασμα δεδομένων του πίνακα `def_othomanoi`
--

INSERT INTO `def_othomanoi` (`id`, `name`, `factor`, `flag`) VALUES
(1, 'Tripolitsa', 4, 1),
(2, 'Nauplio', 3, 1),
(3, 'Patra', 3, 1),
(4, 'Korwni', 3, 1),
(5, 'Monemvasia', 2, 1),
(6, 'Methwni', 2, 1),
(7, 'Korinthos', 2, 1),
(8, 'Navarino', 2, 1);

-- --------------------------------------------------------

--
-- Δομή πίνακα για τον πίνακα `enwsi`
--

CREATE TABLE IF NOT EXISTS `enwsi` (
  `id` int(10) NOT NULL,
  `name` varchar(30) NOT NULL,
  `leader` varchar(30) NOT NULL,
  `leader_rem_rounds` int(30) NOT NULL,
  `vote_rem_rounds` int(30) NOT NULL,
  `break_rem_rounds` int(11) NOT NULL,
  `players` varchar(400) NOT NULL,
  `army` int(30) NOT NULL,
  `army_live` int(10) NOT NULL,
  `gold_percent` decimal(30,2) NOT NULL,
  `farm_percent` decimal(30,2) NOT NULL,
  `craft_percent` decimal(30,2) NOT NULL,
  `dealer_percent` decimal(30,2) NOT NULL,
  `army_percent` decimal(30,2) NOT NULL,
  `gold_live` int(20) NOT NULL,
  `status` int(10) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=latin1;

-- --------------------------------------------------------

--
-- Δομή πίνακα για τον πίνακα `events`
--

CREATE TABLE IF NOT EXISTS `events` (
  `att_player` varchar(30) NOT NULL,
  `def_player` varchar(30) NOT NULL,
  `att_region` varchar(30) NOT NULL,
  `def_region` varchar(30) NOT NULL,
  `army` int(30) NOT NULL,
  `result` int(30) NOT NULL,
  `kind` int(30) NOT NULL,
  `inserton` time NOT NULL,
  `date` date NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=latin1;

-- --------------------------------------------------------

--
-- Δομή πίνακα για τον πίνακα `player`
--

CREATE TABLE IF NOT EXISTS `player` (
  `id` int(10) NOT NULL,
  `name` varchar(20) NOT NULL,
  `picture` mediumblob NOT NULL,
  `surname` varchar(30) NOT NULL,
  `username` varchar(20) NOT NULL,
  `password` varchar(20) NOT NULL,
  `gold` int(20) NOT NULL,
  `military` decimal(30,2) NOT NULL,
  `political` decimal(30,2) NOT NULL,
  `diplomatic` decimal(30,2) NOT NULL,
  `trade` decimal(30,2) NOT NULL,
  `rev_sum` int(30) NOT NULL,
  `cost_sum` int(30) NOT NULL,
  `free_army` int(30) NOT NULL,
  `army_debt_sunasp` int(10) NOT NULL,
  `army_given_sunasp` int(10) NOT NULL,
  `sunasp` varchar(30) NOT NULL,
  `army_debt_enwsi` int(20) NOT NULL,
  `army_given_enwsi` int(20) NOT NULL,
  `enwsi` varchar(30) NOT NULL,
  `enwsi_vote` varchar(30) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=latin1;

-- --------------------------------------------------------

--
-- Δομή πίνακα για τον πίνακα `regions`
--

CREATE TABLE IF NOT EXISTS `regions` (
  `id` int(20) NOT NULL,
  `name` varchar(20) NOT NULL,
  `owner` varchar(20) NOT NULL,
  `farmcon` int(20) NOT NULL,
  `craftcon` int(20) NOT NULL,
  `dealercon` int(20) NOT NULL,
  `farm` int(20) NOT NULL,
  `craft` int(20) NOT NULL,
  `dealer` int(20) NOT NULL,
  `army` int(20) NOT NULL,
  `defence` float NOT NULL,
  `level` float NOT NULL,
  `revenue` int(20) NOT NULL,
  `cost` int(20) NOT NULL,
  `immune` int(20) NOT NULL,
  `offense` int(20) NOT NULL,
  `pol_stab` decimal(30,2) NOT NULL,
  `x` int(20) NOT NULL,
  `y` int(20) NOT NULL,
  `gold` int(20) NOT NULL,
  `cost_bill` int(30) NOT NULL,
  `reg_box` int(50) NOT NULL,
  `ini` int(10) NOT NULL,
  `box_id` int(20) NOT NULL,
  `def_fact` decimal(30,1) NOT NULL,
  `neighbor` varchar(100) NOT NULL,
  `neighbor1` varchar(200) NOT NULL,
  `neighbor2` varchar(200) NOT NULL,
  `neighbor3` varchar(200) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=latin1;

-- --------------------------------------------------------

--
-- Δομή πίνακα για τον πίνακα `reg_treaties`
--

CREATE TABLE IF NOT EXISTS `reg_treaties` (
  `region_a` varchar(30) NOT NULL,
  `region_b` varchar(30) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=latin1;

-- --------------------------------------------------------

--
-- Δομή πίνακα για τον πίνακα `sunaspismos`
--

CREATE TABLE IF NOT EXISTS `sunaspismos` (
  `id` int(10) NOT NULL,
  `name` varchar(20) NOT NULL,
  `leader` varchar(30) NOT NULL,
  `players` varchar(400) NOT NULL,
  `army` int(10) NOT NULL,
  `army_live` int(30) NOT NULL,
  `status` int(10) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=latin1;

-- --------------------------------------------------------

--
-- Δομή πίνακα για τον πίνακα `support`
--

CREATE TABLE IF NOT EXISTS `support` (
  `id` int(30) NOT NULL,
  `attack_id` int(30) NOT NULL,
  `requestor` varchar(30) NOT NULL,
  `supporter` varchar(30) NOT NULL,
  `req_region` varchar(30) NOT NULL,
  `sup_region` varchar(30) NOT NULL,
  `turn` int(30) NOT NULL,
  `army` int(30) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=latin1;

-- --------------------------------------------------------

--
-- Δομή πίνακα για τον πίνακα `support_check`
--

CREATE TABLE IF NOT EXISTS `support_check` (
  `id` int(30) NOT NULL,
  `requestor` varchar(30) NOT NULL,
  `supporter` varchar(30) NOT NULL,
  `req_region` varchar(30) NOT NULL,
  `sup_region` varchar(30) NOT NULL,
  `turn` int(30) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=latin1;

-- --------------------------------------------------------

--
-- Δομή πίνακα για τον πίνακα `treaties`
--

CREATE TABLE IF NOT EXISTS `treaties` (
  `player_a` varchar(30) NOT NULL,
  `player_b` varchar(30) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=latin1;

-- --------------------------------------------------------

--
-- Δομή πίνακα για τον πίνακα `variables`
--

CREATE TABLE IF NOT EXISTS `variables` (
  `def_fact` decimal(30,1) NOT NULL,
  `off_fact` decimal(30,1) NOT NULL,
  `pol_stab` decimal(30,2) NOT NULL,
  `diaspora` int(11) NOT NULL,
  `immune` int(11) NOT NULL,
  `level` int(11) NOT NULL,
  `levelup_fact` decimal(30,1) NOT NULL,
  `craft_per` int(11) NOT NULL,
  `dealer_per` int(11) NOT NULL,
  `reg_dev_ab` int(10) NOT NULL,
  `box_id` int(10) NOT NULL,
  `ini` int(10) NOT NULL,
  `ini_x` int(30) NOT NULL,
  `ini_y` int(30) NOT NULL,
  `ini_regions` int(10) NOT NULL,
  `ini_gold` int(30) NOT NULL,
  `ini_military` decimal(30,2) NOT NULL,
  `ini_trade` decimal(30,2) NOT NULL,
  `ini_diplomatic` decimal(30,2) NOT NULL,
  `ini_political` decimal(30,2) NOT NULL,
  `farm_production` int(30) NOT NULL,
  `craft_production` int(30) NOT NULL,
  `dealer_production` int(30) NOT NULL,
  `army_cost` int(30) NOT NULL,
  `farm_margin` int(30) NOT NULL,
  `craft_margin` int(30) NOT NULL,
  `dealer_margin` int(30) NOT NULL,
  `trade_fact` decimal(30,2) NOT NULL,
  `levelup_cost` int(30) NOT NULL,
  `dev_margin` int(10) NOT NULL,
  `enwsi_gold_contribution` decimal(30,1) NOT NULL,
  `enwsi_lost_regions_percent` decimal(30,2) NOT NULL,
  `enwsi_abandon_percent` decimal(30,2) NOT NULL,
  `enwsi_player_limit` int(30) NOT NULL,
  `sunasp_player_limit` int(30) NOT NULL,
  `sunasp_army_contribution` decimal(30,2) NOT NULL,
  `enwsi_army_contribution` decimal(30,2) NOT NULL,
  `edres_demand_percent` decimal(30,2) NOT NULL,
  `army_percent_sunasp` decimal(10,1) NOT NULL,
  `army_percent_enwsi` decimal(30,1) NOT NULL,
  `gold_percent` decimal(30,2) NOT NULL,
  `farm_percent` decimal(30,2) NOT NULL,
  `craft_percent` decimal(30,2) NOT NULL,
  `dealer_percent` decimal(30,2) NOT NULL,
  `army_percent` decimal(30,2) NOT NULL,
  `enwsi_leader_rounds` int(30) NOT NULL,
  `enwsi_vote_rounds` int(30) NOT NULL,
  `enwsi_break_rounds` int(30) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=latin1;

--
-- Άδειασμα δεδομένων του πίνακα `variables`
--

INSERT INTO `variables` (`def_fact`, `off_fact`, `pol_stab`, `diaspora`, `immune`, `level`, `levelup_fact`, `craft_per`, `dealer_per`, `reg_dev_ab`, `box_id`, `ini`, `ini_x`, `ini_y`, `ini_regions`, `ini_gold`, `ini_military`, `ini_trade`, `ini_diplomatic`, `ini_political`, `farm_production`, `craft_production`, `dealer_production`, `army_cost`, `farm_margin`, `craft_margin`, `dealer_margin`, `trade_fact`, `levelup_cost`, `dev_margin`, `enwsi_gold_contribution`, `enwsi_lost_regions_percent`, `enwsi_abandon_percent`, `enwsi_player_limit`, `sunasp_player_limit`, `sunasp_army_contribution`, `enwsi_army_contribution`, `edres_demand_percent`, `army_percent_sunasp`, `army_percent_enwsi`, `gold_percent`, `farm_percent`, `craft_percent`, `dealer_percent`, `army_percent`, `enwsi_leader_rounds`, `enwsi_vote_rounds`, `enwsi_break_rounds`) VALUES
('0.3', '0.1', '1.00', 10, 10, 1, '0.2', 30, 20, 100, 1, 1, 10, 10, 10, 0, '1.00', '1.00', '1.00', '1.00', 1, 2, 3, 1, 50, 30, 20, '1.00', 1000, 10, '0.5', '0.66', '0.25', 3, 3, '0.33', '0.50', '0.50', '0.3', '0.4', '0.20', '0.20', '0.20', '0.20', '0.40', 12, 5, 5);

/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
