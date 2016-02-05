-- phpMyAdmin SQL Dump
-- version 4.2.7.1
-- http://www.phpmyadmin.net
--
-- Φιλοξενητής: 127.0.0.1
-- Χρόνος δημιουργίας: 14 Ιαν 2015 στις 11:02:47
-- Έκδοση διακομιστή: 5.6.20
-- Έκδοση PHP: 5.5.15

SET SQL_MODE = "NO_AUTO_VALUE_ON_ZERO";
SET time_zone = "+00:00";


/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!40101 SET NAMES utf8 */;

--
-- Βάση δεδομένων: `mapgame`
--
CREATE DATABASE IF NOT EXISTS `mapgame` DEFAULT CHARACTER SET latin1 COLLATE latin1_swedish_ci;
USE `mapgame`;

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
  `kind` int(20) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=latin1;

--
-- Άδειασμα δεδομένων του πίνακα `attacks`
--

INSERT INTO `attacks` (`id`, `att_player`, `def_player`, `att_region`, `def_region`, `distance`, `turn`, `army`, `kind`) VALUES
(1, 'geo7geg', 'panchal', 'Branship Raysword', 'Longdom Stormhard', 20, 7, 5, 1),
(2, 'geo7geg', '', 'Branship Raysword', 'Goodwood Raygold', 8, 6, 2, 1);

-- --------------------------------------------------------

--
-- Δομή πίνακα για τον πίνακα `player`
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
-- Άδειασμα δεδομένων του πίνακα `player`
--

INSERT INTO `player` (`id`, `name`, `surname`, `username`, `password`, `gold`, `military`, `political`, `diplomatic`, `trade`, `rev_sum`, `cost_sum`) VALUES
(1, 'George', 'Gegos', 'geo7geg', '123', 38658, 1, 1, 1, 1, 159, 10),
(2, 'Panagiotis', 'Chaldaios', 'panchal', '123', 26806, 1, 1, 1, 1, 149, 15),
(3, 'Louis', 'Ragkousis', 'louisrag', '123', 27256, 1, 1, 1, 1, 146, 0),
(4, 'Loukas', 'Korakitis', 'loukor', '123', 31180, 1, 1, 1, 1, 168, 0);

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
  `def_fact` int(20) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=latin1;

--
-- Άδειασμα δεδομένων του πίνακα `regions`
--

INSERT INTO `regions` (`id`, `name`, `owner`, `farmcon`, `craftcon`, `dealercon`, `farm`, `craft`, `dealer`, `army`, `defence`, `level`, `revenue`, `cost`, `immune`, `offense`, `pol_stab`, `x`, `y`, `gold`, `cost_bill`, `reg_box`, `ini`, `box_id`, `def_fact`) VALUES
(1, 'Branship Raysword', 'geo7geg', 55, 25, 28, 20, 10, 13, 50, 63, 1, 79, 50, 0, 50, '1.00', 10, 10, 26484, 0, 1, 1, 1, 0),
(2, 'Reidom Arthead', '', 59, 26, 9, 49, 26, 9, 10, 35, 1, 128, 10, 0, 10, '1.00', 4, 8, 17964, 37, 1, 0, 2, 0),
(3, 'Reiship Swanarch', '', 47, 35, 12, 47, 35, 12, 5, 33, 1, 153, 5, 0, 5, '1.00', 4, 18, 28102, 0, 1, 0, 3, 0),
(4, 'Chariron Swanheart', '', 70, 23, 11, 70, 23, 11, 0, 31, 1, 149, 0, 0, 0, '1.00', 14, 3, 20262, 0, 1, 0, 4, 0),
(5, 'Longship Watersilver', '', 56, 27, 25, 56, 27, 25, 0, 32, 1, 185, 0, 0, 0, '1.00', 18, 5, 34140, 0, 1, 0, 5, 0),
(6, 'Brandorn Highleg', '', 32, 37, 26, 32, 37, 26, 0, 28, 1, 184, 0, 0, 0, '1.00', 5, 19, 33956, 0, 1, 0, 6, 0),
(7, 'Charram Stormoak', '', 59, 20, 20, 59, 20, 20, 0, 30, 1, 159, 0, 0, 0, '1.00', 9, 1, 29356, 0, 1, 0, 7, 0),
(8, 'Goodwood Raygold', '', 65, 23, 15, 65, 23, 15, 0, 31, 1, 156, 0, 0, 0, '1.00', 13, 3, 28804, 0, 1, 0, 8, 0),
(9, 'Bellmill Bornhead', '', 41, 33, 16, 41, 33, 16, 0, 27, 1, 155, 0, 0, 0, '1.00', 0, 17, 28620, 0, 1, 0, 9, 0),
(10, 'Reidorn Calfrock', '', 47, 36, 11, 47, 36, 11, 0, 28, 1, 152, 0, 0, 0, '1.00', 4, 19, 28068, 0, 1, 0, 10, 0),
(11, 'Longdom Stormhard', 'panchal', 51, 40, 16, 46, 35, 11, 15, 43, 1, 149, 15, 0, 15, '1.00', -10, 10, 18618, 0, 2, 1, 1, 0),
(12, 'Fithhold Highoak', '', 46, 40, 20, 46, 40, 20, 0, 32, 1, 186, 0, 0, 0, '1.00', -4, 18, 34324, 0, 2, 0, 2, 0),
(13, 'Goodford Bellrock', '', 44, 28, 21, 44, 28, 21, 0, 28, 1, 163, 0, 0, 0, '1.00', -17, 11, 30092, 0, 2, 0, 3, 0),
(14, 'Fasham Lyonsilver', '', 50, 32, 15, 50, 32, 15, 0, 29, 1, 159, 0, 0, 0, '1.00', -13, 13, 29356, 0, 2, 0, 4, 0),
(15, 'Harlwish Stonefort', '', 51, 19, 22, 51, 19, 22, 0, 28, 1, 155, 0, 0, 0, '1.00', -18, 1, 28620, 0, 2, 0, 5, 0),
(16, 'Goodship Farrise', '', 63, 31, 14, 63, 31, 14, 0, 32, 1, 167, 0, 0, 0, '1.00', -2, 9, 30828, 0, 2, 0, 6, 0),
(17, 'Harlford Sparkgold', '', 40, 28, 24, 40, 28, 24, 0, 28, 1, 168, 0, 0, 0, '1.00', -18, 11, 31012, 0, 2, 0, 7, 0),
(18, 'Reimill Raylake', '', 66, 24, 9, 66, 24, 9, 0, 30, 1, 141, 0, 0, 0, '1.00', -3, 19, 26044, 0, 2, 0, 8, 0),
(19, 'Longwood Slygold', '', 52, 27, 24, 52, 27, 24, 0, 31, 1, 178, 0, 0, 0, '1.00', -7, 7, 32852, 0, 2, 0, 9, 0),
(20, 'Riverdom Calflake', '', 54, 34, 10, 54, 34, 10, 0, 29, 1, 152, 0, 0, 0, '1.00', -12, 15, 28068, 0, 2, 0, 10, 0),
(21, 'Chariron Arthorn', 'louisrag', 73, 20, 11, 73, 20, 11, 0, 31, 1, 146, 0, 0, 0, '1.00', -10, -10, 26964, 0, 3, 1, 1, 0),
(22, 'Rivership Calfrise', '', 58, 23, 27, 58, 23, 27, 0, 32, 1, 185, 0, 0, 0, '1.00', -2, -18, 34140, 0, 3, 0, 2, 0),
(23, 'Harlyul Borngold', '', 54, 22, 16, 54, 22, 16, 0, 28, 1, 146, 0, 0, 0, '1.00', -18, -16, 26964, 0, 3, 0, 3, 0),
(24, 'Riverhold Lyonlake', '', 47, 33, 26, 47, 33, 26, 0, 32, 1, 191, 0, 0, 0, '1.00', -4, -8, 35244, 0, 3, 0, 4, 0),
(25, 'Branwood Artgold', '', 68, 20, 15, 68, 20, 15, 0, 31, 1, 153, 0, 0, 0, '1.00', -7, -20, 28252, 0, 3, 0, 5, 0),
(26, 'Langham Calfrise', '', 50, 26, 21, 50, 26, 21, 0, 29, 1, 165, 0, 0, 0, '1.00', -13, -13, 30460, 0, 3, 0, 6, 0),
(27, 'Reistrong Calfsilver', '', 51, 32, 11, 51, 32, 11, 0, 28, 1, 148, 0, 0, 0, '1.00', -16, -5, 27332, 0, 3, 0, 7, 0),
(28, 'Bigdom Highsand', '', 37, 36, 25, 37, 36, 25, 0, 29, 1, 184, 0, 0, 0, '1.00', -12, -3, 33956, 0, 3, 0, 8, 0),
(29, 'Fasdorn Bellrock', '', 49, 35, 25, 49, 35, 25, 0, 33, 1, 194, 0, 0, 0, '1.00', -1, -7, 35796, 0, 3, 0, 9, 0),
(30, 'Charwood Queenhorn', '', 72, 21, 10, 72, 21, 10, 0, 31, 1, 144, 0, 0, 0, '1.00', -7, -19, 26596, 0, 3, 0, 10, 0),
(31, 'Oldworth Bellfort', 'loukor', 54, 30, 18, 54, 30, 18, 0, 31, 1, 168, 0, 0, 0, '1.00', 10, -10, 31012, 0, 4, 1, 1, 0),
(32, 'Reistrong Bornkeep', '', 46, 32, 16, 46, 32, 16, 0, 28, 1, 158, 0, 0, 0, '1.00', 4, -5, 29172, 0, 4, 0, 2, 0),
(33, 'Fithford Highrise', '', 50, 29, 22, 50, 29, 22, 0, 30, 1, 174, 0, 0, 0, '1.00', 11, -11, 32116, 0, 4, 0, 3, 0),
(34, 'Culstrong Piercekeep', '', 57, 32, 16, 57, 32, 16, 0, 32, 1, 169, 0, 0, 0, '1.00', 15, -9, 31196, 0, 4, 0, 4, 0),
(35, 'Lightdorn Bellhard', '', 52, 23, 17, 52, 23, 17, 0, 28, 1, 149, 0, 0, 0, '1.00', 19, -7, 27516, 0, 4, 0, 5, 0),
(36, 'Culwood Darwrock', '', 66, 21, 16, 66, 21, 16, 0, 31, 1, 156, 0, 0, 0, '1.00', 13, -19, 28804, 0, 4, 0, 6, 0),
(37, 'Reimill Swanhorn', '', 48, 34, 12, 48, 34, 12, 0, 28, 1, 152, 0, 0, 0, '1.00', 4, -3, 28068, 0, 4, 0, 7, 0),
(38, 'Charclap Bellarch', '', 49, 22, 20, 49, 22, 20, 0, 27, 1, 153, 0, 0, 0, '1.00', 1, -15, 28252, 0, 4, 0, 8, 0),
(39, 'Branham Stormsand', '', 55, 25, 15, 55, 25, 15, 0, 28, 1, 150, 0, 0, 0, '1.00', 5, -13, 27700, 0, 4, 0, 9, 0),
(40, 'Longdorn Kingheart', '', 45, 38, 26, 45, 38, 26, 0, 33, 1, 199, 0, 0, 0, '1.00', 19, -5, 36716, 0, 4, 0, 10, 0);

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
-- Δομή πίνακα για τον πίνακα `treaties`
--

CREATE TABLE IF NOT EXISTS `treaties` (
  `player_a` varchar(30) NOT NULL,
  `player_b` varchar(30) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=latin1;

/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
