-- phpMyAdmin SQL Dump
-- version 4.2.7.1
-- http://www.phpmyadmin.net
--
-- Host: 127.0.0.1
-- Generation Time: Feb 11, 2015 at 08:34 AM
-- Server version: 5.6.20
-- PHP Version: 5.5.15

SET SQL_MODE = "NO_AUTO_VALUE_ON_ZERO";
SET time_zone = "+00:00";


/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!40101 SET NAMES utf8 */;

--
-- Database: `mapgame`
--
CREATE DATABASE IF NOT EXISTS `mapgame1` DEFAULT CHARACTER SET latin1 COLLATE latin1_swedish_ci;
USE `mapgame1`;

-- --------------------------------------------------------

--
-- Table structure for table `agreement`
--

CREATE TABLE IF NOT EXISTS `agreement` (
  `id` int(30) NOT NULL,
  `player_a` varchar(30) NOT NULL,
  `player_b` varchar(30) NOT NULL,
  `region_a` varchar(30) NOT NULL,
  `region_b` varchar(30) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=latin1;

--
-- Dumping data for table `agreement`
--

INSERT INTO `agreement` (`id`, `player_a`, `player_b`, `region_a`, `region_b`) VALUES
(2, 'geo7geg', '', 'Branship Raysword', 'Bigdom Highsand'),
(4, 'geo7geg', '', 'Branship Raysword', 'Charram Stormoak'),
(1, 'geo7geg', '', 'Branship Raysword', 'Harlwish Stonefort');

-- --------------------------------------------------------

--
-- Table structure for table `attacks`
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
  `kind` int(20) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=latin1;

--
-- Dumping data for table `attacks`
--

INSERT INTO `attacks` (`id`, `att_player`, `def_player`, `att_region`, `def_region`, `distance`, `turn`, `army`, `kind`) VALUES
(3, 'geo7geg', '', 'Branship Raysword', 'Goodford Bellrock', 27, 10, 5, 1),
(6, 'panchal', 'geo7geg', 'Longdom Stormhard', 'Chariron Swanheart', 25, 10, 7, 1);

--
-- Triggers `attacks`
--
DELIMITER //
CREATE TRIGGER `attack_events` AFTER INSERT ON `attacks`
 FOR EACH ROW INSERT INTO events (att_player,def_player,att_region,def_region,army,inserton,date) VALUES (NEW.att_player,NEW.def_player,NEW.att_region,NEW.def_region,NEW.army,NOW(),NOW())
//
DELIMITER ;

-- --------------------------------------------------------

--
-- Table structure for table `events`
--

CREATE TABLE IF NOT EXISTS `events` (
  `att_player` varchar(30) NOT NULL,
  `def_player` varchar(30) NOT NULL,
  `att_region` varchar(30) NOT NULL,
  `def_region` varchar(30) NOT NULL,
  `army` int(30) NOT NULL,
  `result` int(30) NOT NULL,
  `inserton` time NOT NULL,
  `date` date NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=latin1;

--
-- Dumping data for table `events`
--

INSERT INTO `events` (`att_player`, `def_player`, `att_region`, `def_region`, `army`, `result`, `inserton`, `date`) VALUES
('geo7geg', '', 'Branship Raysword', 'Longship Watersilver', 5, 0, '10:10:44', '2015-02-10'),
('geo7geg', '', 'Branship Raysword', 'Bellmill Bornhead', 5, 0, '10:10:54', '2015-02-10'),
('geo7geg', '', 'Branship Raysword', 'Reidorn Calfrock', 30, 0, '10:11:08', '2015-02-10'),
('geo7geg', 'panchal', 'Branship Raysword', 'Longdom Stormhard', 4, 0, '10:12:17', '2015-02-10'),
('geo7geg', '', 'Branship Raysword', 'Harlwish Stonefort', 0, 0, '10:22:23', '2015-02-10'),
('geo7geg', '', 'Branship Raysword', 'Longship Watersilver', 0, 0, '10:22:25', '2015-02-10'),
('geo7geg', '', 'Branship Raysword', 'Reidorn Calfrock', 0, 1, '10:43:55', '2015-02-10'),
('geo7geg', '', 'Branship Raysword', 'Bellmill Bornhead', 0, 0, '11:37:49', '2015-02-10'),
('geo7geg', '', 'Branship Raysword', 'Longship Watersilver', 5, 0, '11:38:04', '2015-02-10'),
('geo7geg', '', 'Branship Raysword', 'Brandorn Highleg', 5, 0, '11:38:10', '2015-02-10'),
('geo7geg', '', 'Branship Raysword', 'Goodford Bellrock', 5, 0, '13:12:40', '2015-02-10'),
('panchal', '', 'Longdom Stormhard', 'Fithhold Highoak', 5, 0, '14:41:11', '2015-02-10'),
('panchal', 'geo7geg', 'Longdom Stormhard', 'Chariron Swanheart', 7, 0, '14:41:26', '2015-02-10'),
('geo7geg', '', 'Branship Raysword', 'Longship Watersilver', 0, 0, '14:41:32', '2015-02-10'),
('geo7geg', 'panchal', 'Branship Raysword', 'Longdom Stormhard', 0, 0, '14:44:00', '2015-02-10'),
('geo7geg', '', 'Branship Raysword', 'Brandorn Highleg', 0, 0, '14:44:02', '2015-02-10'),
('geo7geg', '', 'Branship Raysword', 'Longship Watersilver', 17, 0, '15:23:52', '2015-02-10'),
('panchal', '', 'Longdom Stormhard', 'Fithhold Highoak', 0, 0, '15:25:16', '2015-02-10'),
('geo7geg', '', 'Branship Raysword', 'Longship Watersilver', 0, 1, '15:25:34', '2015-02-10');

-- --------------------------------------------------------

--
-- Table structure for table `player`
--

CREATE TABLE IF NOT EXISTS `player` (
  `id` int(10) NOT NULL,
  `name` varchar(20) NOT NULL,
  `surname` varchar(30) NOT NULL,
  `username` varchar(20) NOT NULL,
  `password` varchar(20) NOT NULL,
  `gold` int(20) NOT NULL,
  `military` float NOT NULL,
  `political` float NOT NULL,
  `diplomatic` float NOT NULL,
  `trade` float NOT NULL,
  `rev_sum` int(30) NOT NULL,
  `cost_sum` int(30) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=latin1;

--
-- Dumping data for table `player`
--

INSERT INTO `player` (`id`, `name`, `surname`, `username`, `password`, `gold`, `military`, `political`, `diplomatic`, `trade`, `rev_sum`, `cost_sum`) VALUES
(1, 'George', 'Gegos', 'geo7geg', '123', 94012, 2, 1, 1, 1, 995, 70),
(2, 'Panagiotis', 'Chaldaios', 'panchal', '123', 57404, 1, 1, 1, 1, 114, 28),
(3, 'Louis', 'Ragkousis', 'louisrag', '123', 64392, 1, 1, 1, 1, 126, 14),
(4, 'Loukas', 'Korakitis', 'loukor', '123', 85394, 1, 1, 1, 1, 239, 39),
(5, 'Mihalis', 'Koukos', 'mih_kouk', '123', 53580, 1, 1, 1, 1, 191, 0);

-- --------------------------------------------------------

--
-- Table structure for table `regions`
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
  `def_fact` int(20) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=latin1;

--
-- Dumping data for table `regions`
--

INSERT INTO `regions` (`id`, `name`, `owner`, `farmcon`, `craftcon`, `dealercon`, `farm`, `craft`, `dealer`, `army`, `defence`, `level`, `revenue`, `cost`, `immune`, `offense`, `pol_stab`, `x`, `y`, `gold`, `cost_bill`, `reg_box`, `ini`, `box_id`, `def_fact`) VALUES
(1, 'Branship Raysword', 'geo7geg', 55, 25, 28, 19, 13, 13, 48, 62, 1, 84, 48, 0, 48, '0.42', 10, 10, 37005, 118, 1, 1, 1, 0),
(2, 'Reidom Arthead', 'geo7geg', 59, 26, 9, 47, 26, 9, 12, 37, 1, 126, 12, 0, 12, '1.00', 4, 8, 11559, 0, 1, 0, 2, 0),
(3, 'Reiship Swanarch', 'geo7geg', 47, 35, 12, 47, 35, 12, 0, 28, 1, 153, 0, 0, 0, '1.00', 4, 18, 5508, 0, 1, 0, 3, 0),
(4, 'Chariron Swanheart', 'geo7geg', 70, 23, 11, 60, 23, 11, 10, 38, 1, 139, 10, 0, 10, '1.00', 14, 3, 11859, 0, 1, 0, 4, 0),
(5, 'Longship Watersilver', 'geo7geg', 56, 27, 25, 56, 27, 25, 0, 32, 1, 185, 0, 0, 0, '1.00', 18, 5, 185, 0, 1, 0, 5, 0),
(6, 'Brandorn Highleg', '', 32, 37, 26, 32, 37, 26, 0, 28, 1, 184, 0, 0, 0, '1.00', 5, 19, 85844, 0, 1, 0, 6, 0),
(7, 'Charram Stormoak', '', 59, 20, 20, 59, 20, 20, 0, 30, 1, 159, 0, 0, 0, '1.00', 9, 1, 74194, 0, 1, 0, 7, 0),
(8, 'Goodwood Raygold', 'geo7geg', 65, 23, 15, 65, 23, 15, 0, 31, 1, 156, 0, 0, 0, '1.00', 13, 3, 11856, 0, 1, 0, 8, 0),
(9, 'Bellmill Bornhead', '', 41, 33, 16, 41, 33, 16, 0, 27, 1, 155, 0, 0, 0, '1.00', 0, 17, 72330, 0, 1, 0, 9, 0),
(10, 'Reidorn Calfrock', 'geo7geg', 47, 36, 11, 47, 36, 11, 0, 28, 1, 152, 0, 0, 0, '1.00', 4, 19, 3800, 0, 1, 0, 10, 0),
(11, 'Longdom Stormhard', 'panchal', 51, 40, 16, 31, 31, 7, 28, 49, 1, 114, 28, 0, 28, '0.64', -10, 10, 49136, 63, 2, 1, 1, 0),
(12, 'Fithhold Highoak', '', 46, 40, 20, 46, 40, 20, 10, 42, 1, 186, 10, 0, 10, '1.00', -4, 18, 84256, 0, 2, 0, 2, 0),
(13, 'Goodford Bellrock', '', 44, 28, 21, 44, 28, 21, 0, 28, 1, 163, 0, 0, 0, '1.00', -17, 11, 76058, 0, 2, 0, 3, 0),
(14, 'Fasham Lyonsilver', '', 50, 32, 15, 50, 32, 15, 0, 29, 1, 159, 0, 0, 0, '1.00', -13, 13, 74194, 0, 2, 0, 4, 0),
(15, 'Harlwish Stonefort', '', 51, 19, 22, 51, 19, 22, 0, 28, 1, 155, 0, 0, 0, '1.00', -18, 1, 72330, 0, 2, 0, 5, 0),
(16, 'Goodship Farrise', '', 63, 31, 14, 63, 31, 14, 0, 32, 1, 167, 0, 0, 0, '1.00', -2, 9, 77922, 0, 2, 0, 6, 0),
(17, 'Harlford Sparkgold', '', 40, 28, 24, 40, 28, 24, 0, 28, 1, 168, 0, 0, 0, '1.00', -18, 11, 78388, 0, 2, 0, 7, 0),
(18, 'Reimill Raylake', '', 66, 24, 9, 66, 24, 9, 0, 30, 1, 141, 0, 0, 0, '1.00', -3, 19, 65806, 0, 2, 0, 8, 0),
(19, 'Longwood Slygold', '', 52, 27, 24, 52, 27, 24, 0, 31, 1, 178, 0, 0, 0, '1.00', -7, 7, 83048, 0, 2, 0, 9, 0),
(20, 'Riverdom Calflake', '', 54, 34, 10, 54, 34, 10, 0, 29, 1, 152, 0, 0, 0, '1.00', -12, 15, 70932, 0, 2, 0, 10, 0),
(21, 'Chariron Arthorn', 'louisrag', 73, 20, 11, 63, 18, 9, 14, 41, 1, 126, 14, 0, 14, '1.00', -10, -10, 63954, 0, 3, 1, 1, 0),
(22, 'Rivership Calfrise', '', 58, 23, 27, 58, 23, 27, 0, 32, 1, 185, 0, 0, 0, '1.00', -2, -18, 86310, 0, 3, 0, 2, 0),
(23, 'Harlyul Borngold', '', 54, 22, 16, 54, 22, 16, 0, 28, 1, 146, 0, 0, 0, '1.00', -18, -16, 68136, 0, 3, 0, 3, 0),
(24, 'Riverhold Lyonlake', '', 47, 33, 26, 47, 33, 26, 0, 32, 1, 191, 0, 0, 0, '1.00', -4, -8, 89106, 0, 3, 0, 4, 0),
(25, 'Branwood Artgold', '', 68, 20, 15, 68, 20, 15, 0, 31, 1, 153, 0, 0, 0, '1.00', -7, -20, 71398, 0, 3, 0, 5, 0),
(26, 'Langham Calfrise', '', 50, 26, 21, 50, 26, 21, 0, 29, 1, 165, 0, 0, 0, '1.00', -13, -13, 76990, 0, 3, 0, 6, 0),
(27, 'Reistrong Calfsilver', '', 51, 32, 11, 51, 32, 11, 0, 28, 1, 148, 0, 0, 0, '1.00', -16, -5, 69068, 0, 3, 0, 7, 0),
(28, 'Bigdom Highsand', '', 37, 36, 25, 37, 36, 25, 0, 29, 1, 184, 0, 0, 0, '1.00', -12, -3, 85844, 0, 3, 0, 8, 0),
(29, 'Fasdorn Bellrock', '', 49, 35, 25, 49, 35, 25, 0, 33, 1, 194, 0, 0, 0, '1.00', -1, -7, 90504, 0, 3, 0, 9, 0),
(30, 'Charwood Queenhorn', '', 72, 21, 10, 72, 21, 10, 0, 31, 1, 144, 0, 0, 0, '1.00', -7, -19, 67204, 0, 3, 0, 10, 0),
(31, 'Oldworth Bellfort', 'loukor', 54, 30, 18, 44, 20, 8, 30, 52, 1, 108, 30, 0, 30, '1.00', 10, -10, 57283, 0, 4, 1, 1, 0),
(32, 'Reistrong Bornkeep', 'loukor', 46, 32, 16, 46, 32, 7, 9, 34, 1, 131, 9, 0, 9, '1.00', 4, -5, 27716, 0, 4, 0, 2, 0),
(33, 'Fithford Highrise', '', 50, 29, 22, 50, 29, 22, 0, 30, 1, 174, 0, 0, 0, '1.00', 11, -11, 81184, 0, 4, 0, 3, 0),
(34, 'Culstrong Piercekeep', '', 57, 32, 16, 57, 32, 16, 0, 32, 1, 169, 0, 0, 0, '1.00', 15, -9, 78854, 0, 4, 0, 4, 0),
(35, 'Lightdorn Bellhard', '', 52, 23, 17, 52, 23, 17, 0, 28, 1, 149, 0, 0, 0, '1.00', 19, -7, 69534, 0, 4, 0, 5, 0),
(36, 'Culwood Darwrock', '', 66, 21, 16, 66, 21, 16, 0, 31, 1, 156, 0, 0, 0, '1.00', 13, -19, 72796, 0, 4, 0, 6, 0),
(37, 'Reimill Swanhorn', '', 48, 34, 12, 48, 34, 12, 0, 28, 1, 152, 0, 0, 0, '1.00', 4, -3, 70932, 0, 4, 0, 7, 0),
(38, 'Charclap Bellarch', '', 49, 22, 20, 49, 22, 20, 0, 27, 1, 153, 0, 0, 0, '1.00', 1, -15, 71398, 0, 4, 0, 8, 0),
(39, 'Branham Stormsand', '', 55, 25, 15, 55, 25, 15, 0, 28, 1, 150, 0, 0, 0, '1.00', 5, -13, 70000, 0, 4, 0, 9, 0),
(40, 'Longdorn Kingheart', '', 45, 38, 26, 45, 38, 26, 0, 33, 1, 199, 0, 0, 0, '1.00', 19, -5, 92834, 0, 4, 0, 10, 0),
(41, 'Bellstrong Waterrock', 'mih_kouk', 48, 40, 21, 48, 40, 21, 0, 33, 1, 191, 0, 0, 0, '1.00', 30, -10, 53389, 0, 5, 1, 1, 0),
(42, 'Goodvill Lyonoak', '', 56, 23, 21, 56, 23, 21, 0, 30, 1, 165, 0, 0, 0, '1.00', 30, -17, 46135, 0, 5, 0, 2, 0),
(43, 'Branford Highfort', '', 40, 29, 26, 40, 29, 26, 0, 28, 1, 176, 0, 0, 0, '1.00', 25, -9, 49204, 0, 5, 0, 3, 0),
(44, 'Fasram Darwoak', '', 47, 32, 20, 47, 32, 20, 0, 30, 1, 171, 0, 0, 0, '1.00', 29, -7, 47809, 0, 5, 0, 4, 0),
(45, 'Longwood Bellgold', '', 53, 35, 15, 53, 35, 15, 0, 31, 1, 168, 0, 0, 0, '1.00', 33, -6, 46972, 0, 5, 0, 5, 0),
(46, 'Belldom Bornlake', '', 49, 25, 16, 49, 25, 16, 0, 27, 1, 147, 0, 0, 0, '1.00', 20, -12, 41113, 0, 5, 0, 6, 0),
(47, 'Reivill Calfleg', '', 55, 28, 11, 55, 28, 11, 0, 28, 1, 144, 0, 0, 0, '1.00', 24, -10, 40276, 0, 5, 0, 7, 0),
(48, 'Burdorn Bornhead', '', 56, 30, 23, 56, 30, 23, 0, 33, 1, 185, 0, 0, 0, '1.00', 39, -12, 51715, 0, 5, 0, 8, 0),
(49, 'Reivill Slyelf', '', 57, 24, 19, 57, 24, 19, 0, 30, 1, 162, 0, 0, 0, '1.00', 30, -16, 45298, 0, 5, 0, 9, 0),
(50, 'Goodiron Stormhorn', '', 64, 27, 13, 64, 27, 13, 0, 31, 1, 157, 0, 0, 0, '1.00', 34, -14, 43903, 0, 5, 0, 10, 0);

-- --------------------------------------------------------

--
-- Table structure for table `reg_treaties`
--

CREATE TABLE IF NOT EXISTS `reg_treaties` (
  `region_a` varchar(30) NOT NULL,
  `region_b` varchar(30) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=latin1;

-- --------------------------------------------------------

--
-- Table structure for table `treaties`
--

CREATE TABLE IF NOT EXISTS `treaties` (
  `player_a` varchar(30) NOT NULL,
  `player_b` varchar(30) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=latin1;

--
-- Dumping data for table `treaties`
--

INSERT INTO `treaties` (`player_a`, `player_b`) VALUES
('geo7geg', 'louisrag'),
('louisrag', 'geo7geg');

/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
