-- phpMyAdmin SQL Dump
-- version 4.2.7.1
-- http://www.phpmyadmin.net
--
-- Host: 127.0.0.1
-- Generation Time: Mar 02, 2015 at 10:48 AM
-- Server version: 5.6.20
-- PHP Version: 5.5.15

SET SQL_MODE = "NO_AUTO_VALUE_ON_ZERO";
SET time_zone = "+00:00";


/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!40101 SET NAMES utf8 */;

--
-- Database: `mapgame1`
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
-- Triggers `attacks`
--
DELIMITER //
CREATE TRIGGER `attack_events` AFTER INSERT ON `attacks`
 FOR EACH ROW INSERT INTO events (att_player,def_player,att_region,def_region,army,kind,inserton,date) VALUES (NEW.att_player,NEW.def_player,NEW.att_region,NEW.def_region,NEW.army,NEW.kind,NOW(),NOW())
//
DELIMITER ;

-- --------------------------------------------------------

--
-- Table structure for table `def_othomanoi`
--

CREATE TABLE IF NOT EXISTS `def_othomanoi` (
  `id` int(10) NOT NULL,
  `name` varchar(200) NOT NULL,
  `factor` int(10) NOT NULL,
  `flag` int(5) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=latin1;

--
-- Dumping data for table `def_othomanoi`
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
-- Table structure for table `events`
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
-- Table structure for table `player`
--

CREATE TABLE IF NOT EXISTS `player` (
  `id` int(10) NOT NULL,
  `name` varchar(20) NOT NULL,
  `surname` varchar(30) NOT NULL,
  `username` varchar(20) NOT NULL,
  `password` varchar(20) NOT NULL,
  `gold` int(20) NOT NULL,
  `military` decimal(30,2) NOT NULL,
  `political` float NOT NULL,
  `diplomatic` float NOT NULL,
  `trade` float NOT NULL,
  `rev_sum` int(30) NOT NULL,
  `cost_sum` int(30) NOT NULL,
  `free_army` int(30) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=latin1;

--
-- Dumping data for table `player`
--

INSERT INTO `player` (`id`, `name`, `surname`, `username`, `password`, `gold`, `military`, `political`, `diplomatic`, `trade`, `rev_sum`, `cost_sum`, `free_army`) VALUES
(1, 'Petrompeis', 'Mauromixalis', 'Mauromixalis', '1821', 0, '2.00', 1, 1, 1, 0, 0, 0),
(2, 'Theodwros', 'Kolokotrwnis', 'Kolokotrwnis', '1821', 0, '3.00', 1, 1, 1, 0, 0, 0),
(3, 'Andreas', 'Lontos', 'Lontos', '1821', 0, '1.00', 1, 1, 1, 0, 0, 0),
(4, 'Laskarina', 'Mpoumpoulina', 'Mpoumpoulina', '1821', 0, '1.00', 1, 1, 1, 0, 0, 0),
(5, 'Georgios', 'Papaflessas', 'Papaflessas', '1821', 0, '2.00', 1, 1, 1, 0, 0, 0);

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
  `def_fact` int(20) NOT NULL,
  `neighbor` varchar(100) NOT NULL,
  `neighbor1` varchar(200) NOT NULL,
  `neighbor2` varchar(200) NOT NULL,
  `neighbor3` varchar(200) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=latin1;

--
-- Dumping data for table `regions`
--

INSERT INTO `regions` (`id`, `name`, `owner`, `farmcon`, `craftcon`, `dealercon`, `farm`, `craft`, `dealer`, `army`, `defence`, `level`, `revenue`, `cost`, `immune`, `offense`, `pol_stab`, `x`, `y`, `gold`, `cost_bill`, `reg_box`, `ini`, `box_id`, `def_fact`, `neighbor`, `neighbor1`, `neighbor2`, `neighbor3`) VALUES
(1, 'Patra', '', 40, 40, 40, 40, 40, 40, 0, 36, 1, 240, 0, 0, 0, '1.00', -134, 314, 0, 0, 1, 1, 1, 0, 'Kalavruta,Vostitsa,Katw Axaia', 'Karytaina,Korinthos,Lexaina,Purgos,Fanari', 'Aigina,Androusa,Gastouni,Leontari,Lipovitsi,Megara,Prastos,Spetses,Tripolitsa', 'Agios Petros,Arkadia,Argos,Katw Naxagies,Korwni,Koutsouk Mani,Mani,Methwni,Emplakika,Monemvasia,Mystras,Navarino,Nauplio,Nhsi,Poros,Salamina,Skala,Ydra'),
(2, 'Vostitsa', 'Lontos', 30, 40, 20, 30, 40, 20, 0, 27, 1, 170, 0, 0, 0, '1.00', -91, 309, 0, 0, 1, 0, 2, 0, 'Patra,Kalavruta,Korinthos,Katw Axaia', 'Aigina,Argos,Gastouni,Karytaina,Leontari,Lexaina,Lipovitsi,Megara,Tripolitsa,Fanari', 'Agios Petros,Arkadia,Koutsouk Mani,Androusa,Mani,Emplakika,Mystras,Nauplio,Prastos,Purgos,Salamina', 'Katw Naxagies,Korwni,Methwni,Monemvasia,Navarino,Nhsi,Poros,Skala,Spetses,Ydra'),
(3, 'Kalavruta', '', 50, 40, 30, 50, 40, 30, 0, 36, 1, 220, 0, 0, 0, '1.00', -70, 244, 0, 0, 1, 0, 3, 0, 'Vostitsa,Tripolitsa,Karytaina,Gastouni,Lipovitsi,Katw Axaia', 'Agios Petros,Argos,Karytaina,Lexaina,Megara,Mystras,Nauplio,Patra,Purgos,Fanari', 'Aigina,Androusa,Arkadia,Koutsouk Mani,Leontari,Emplakika,Spetses', 'Katw Naxagies,Korwni,Mani,Methwni,Monemvasia,Navarino,Nhsi,Poros,Prastos,Salamina,Skala,Ydra'),
(4, 'Katw Axaia', '', 40, 30, 20, 40, 30, 20, 0, 27, 1, 160, 0, 0, 0, '1.00', -202, 269, 0, 0, 1, 0, 4, 0, 'Vostitsa,Patra,Kalavruta,Gastouni,Lexaina', 'Karytaina,Lipovitsi,Purgos,Tripolitsa,Fanari,Korinthos', 'Arkadia,Agios Petros,Aigina,Androusa,Argos,Koutsouk Mani,Leontari,Emplakika,Nauplio,Nhsi', 'Katw Naxagies,Korwni,Mani,Megara,Methwni,Monemvasia,Mystras,Navarino,Poros,Prastos,Salamina,Skala,Spetses,Ydra'),
(5, 'Lexaina', '', 20, 0, 10, 20, 0, 10, 0, 9, 1, 50, 0, 0, 0, '1.00', -253, 234, 0, 0, 1, 0, 5, 0, 'Gastouni,Katw Axaia,', 'Vostitsa,Kalavruta,Karytaina,Lipovitsi,Patra,Purgos,Fanari', 'Aigina,Androusa,Korinthos,Leontari,Emplakika,Mystras,Nauplio,Nhsi,Prastos,Tripolitsa', 'Agios Petros,Argos,Arkadia,Katw Naxagies,Korwni,Koutsouk Mani,Mani,Megara,Methwni,Monemvasia,Navarino,Poros,Salamina,Skala,Spetses,Ydra'),
(6, 'Gastouni', '', 30, 10, 20, 30, 10, 20, 0, 18, 1, 110, 0, 0, 0, '1.00', -268, 188, 0, 0, 1, 0, 6, 0, 'Kalavruta,Fanari,Purgos,Katw Axaia,Lexaina,Lipovitsi', 'Arkadia,Vostitsa,Leontari,Patra,Tripolitsa', 'Agios Petros,Aigina,Androusa,Argos,Korinthos,Koutsouk Mani,Emplakika,Mystras,Navarino,Nauplio,Nhsi', 'Katw Naxagies,Korwni,Mani,Megara,Methwni,Monemvasia,Poros,Prastos,Salamina,Skala,Spetses,Ydra'),
(7, 'Purgos', '', 20, 20, 20, 20, 20, 20, 0, 18, 1, 120, 0, 0, 0, '1.00', -224, 154, 0, 0, 1, 0, 7, 0, 'Fanari', 'Kalavruta,Karytaina,Leontari,Lexaina,Patra', 'Androusa,Arkadia,Vostitsa,Korinthos,Lipovitsi,Mystras,Navarino,Nhsi,Tripolitsa', 'Agios Petros,Aigina,Argos,Gastouni,Katw Axaia,Katw Naxagies,Korwni,Koutsouk Mani,Mani,Megara,Methwni,Emplakika,Monemvasia,Nauplio,Poros,Prastos,Salamina,Skala,Spetses,Ydra'),
(8, 'Fanari', '', 40, 10, 10, 40, 10, 10, 0, 18, 1, 90, 0, 0, 0, '1.00', -168, 91, 0, 0, 1, 0, 8, 0, 'Gastouni,Karytaina,Leontari,Purgos', 'Androusa,Arkadia,Vostitsa,Katw Axaia,Kalavruta,Koutsouk Mani,Lexaina,Lipovitsi,Methwni,Emplakika,Patra,Tripolitsa', 'Agios Petros,Argos,Korinthos,Korwni,Mani,Monemvasia,Mystras,Navarino,Nauplio,Nhsi,Prastos,Salamina,Skala', 'Aigina,Katw Naxagies,Megara,Poros,Spetses,Ydra'),
(9, 'Arkadia', 'Papaflessas', 40, 30, 20, 40, 30, 20, 0, 27, 1, 160, 0, 0, 0, '1.00', -147, 21, 0, 0, 1, 0, 9, 0, 'Fanari,Androusa,Emplakika,Navarino,Leontari', 'Purgos,Gastouni,Karytaina,Korwni,Lipovitsi,Methwni,Emplakika,Navarino,Nhsi', 'Agios Petros,Argos,Vostitsa,Katw Axaia,Kalavruta,Korinthos,Koutsouk Mani,Mystras,Nauplio,Prastos,Tripolitsa', 'Katw Naxagies,Aigina,Lexaina,Mani,Megara,Monemvasia,Patra,Poros,Salamina,Skala,Spetses,Ydra'),
(10, 'Navarino', '', 40, 30, 20, 40, 30, 20, 0, 36, 1, 160, 0, 0, 0, '1.00', -157, -107, 0, 0, 1, 0, 10, 0, 'Androusa,Methwni,Salamina', 'Arkadia,Korwni,Koutsouk Mani,Leontari,Mani,Emplakika,Nhsi', 'Gastouni,Karytaina,Monemvasia,Mystras,Nauplio,Prastos,Purgos,Skala,Spetses,Tripolitsa,Fanari', 'Agios Petros,Aigina,Argos,Vostitsa,Katw Axaia,Kalavruta,Katw Naxagies,Korinthos,Lexaina,Lipovitsi,Megara,Patra,Poros,Ydra'),
(11, 'Methwni', '', 30, 15, 0, 30, 15, 0, 0, 14, 1, 60, 0, 0, 0, '1.00', -144, -130, 0, 0, 2, 1, 1, 0, 'Androusa,Korwni,Navarino,Nhsi', 'Arkadia,Koutsouk Mani,Mani,Emplakika,Mystras,Fanari', 'Leontari,Monemvasia,Nauplio,Prastos,Skala,Tripolitsa', 'Agios Petros,Aigina,Argos,Vostitsa,Gastouni,Katw Axaia,Kalavruta,Karytaina,Katw Naxagies,Korinthos,Lexaina,Lipovitsi,Megara,Patra,Poros,Purgos,Salamina,Spetses,Ydra'),
(12, 'Korwni', '', 40, 50, 30, 40, 50, 30, 0, 36, 1, 230, 0, 0, 0, '1.00', -115, -135, 0, 0, 2, 0, 2, 0, 'Androusa,Methwni,Nhsi', 'Arkadia,Koutsouk Mani,Mani,Emplakika,Mystras,Navarino', 'Aigina,Leontari,Monemvasia,Nauplio,Prastos,Skala,Tripolitsa,Fanari', 'Agios Petros,Argos,Vostitsa,Gastouni,Katw Axaia,Kalavruta,Karytaina,Katw Naxagies,Korinthos,Lexaina,Lipovitsi,Megara,Patra,Poros,Purgos,Salamina,Spetses,Ydra'),
(13, 'Nhsi', '', 10, 10, 10, 10, 10, 10, 0, 9, 1, 60, 0, 0, 0, '1.00', -95, -59, 0, 0, 2, 0, 3, 0, 'Androusa,Korwni,Koutsouk Mani,Mani,Methwni', 'Emplakika,Monemvasia,Mystras,Navarino,Skala,', 'Agios Petros,Aigina,Arkadia,Gastouni,Katw Axaia,Karytaina,Katw Naxagies,Korinthos,Lexaina,Lipovitsi,Megara,Nauplio,Prastos,Purgos,Tripolitsa,Fanari', 'Argos,Vostitsa,Kalavruta,Leontari,Patra,Poros,Salamina,Spetses,Ydra'),
(14, 'Emplakika', '', 30, 0, 0, 30, 0, 0, 0, 9, 1, 30, 0, 0, 0, '1.00', -90, -18, 0, 0, 2, 0, 4, 0, 'Androusa,Koutsouk Mani,Leontari', 'Arkadia,Korwni,Lipovitsi,Mani,Methwni,Mystras,Navarino,Nhsi,Fanari', 'Agios Petros,Aigina,Argos,Vostitsa,Kalavruta,Katw Naxagies,Korinthos,Lexaina,Megara,Monemvasia,Nauplio,Prastos,Skala,Spetses,Tripolitsa', 'Gastouni,Katw Axaia,Karytaina,Patra,Poros,Purgos,Salamina,Ydra'),
(15, 'Koutsouk Mani', '', 20, 10, 0, 20, 10, 0, 0, 9, 1, 40, 0, 0, 0, '1.00', -38, -57, 0, 0, 2, 0, 5, 0, 'Androusa,Leontari,Mani,Emplakika,Mystras,Nhsi', 'Agios Petros,Korwni,Methwni,Monemvasia,Navarino,Skala,Tripolitsa,Fanari', 'Aigina,Argos,Arkadia,Vostitsa,Gastouni,Katw Axaia,Kalavruta,Karytaina,Katw Naxagies,Lipovitsi,Nauplio,Prastos,Spetses', 'Korinthos,Lexaina,Megara,Patra,Poros,Purgos,Salamina,Ydra'),
(16, 'Mani', 'Mauromixalis', 40, 40, 10, 40, 40, 10, 0, 35, 1, 150, 0, 0, 0, '1.00', 29, -184, 0, 0, 2, 0, 6, 0, 'Koutsouk Mani,Leontari,Mystras,Nhsi,Skala', 'Agios Petros,Androusa,Korwni,Methwni,Emplakika,Monemvasia,Navarino', 'Aigina,Argos,Vostitsa,Nauplio,Prastos,Spetses,Tripolitsa,Fanari', 'Arkadia,Gastouni,Katw Axaia,Kalavruta,Karytaina,Katw Naxagies,Korinthos,Lexaina,Lipovitsi,Megara,Patra,Poros,Purgos,Salamina,Ydra'),
(17, 'Skala', '', 25, 10, 10, 25, 10, 10, 0, 14, 1, 75, 0, 0, 0, '1.00', 98, -124, 0, 0, 2, 0, 7, 0, 'Mani,Mystras', 'Agios Petros,Koutsouk Mani,Leontari,Monemvasia,Nhsi,Prastos', 'Aigina,Androusa,Korwni,Methwni,Emplakika,Navarino,Nauplio,Spetses,Tripolitsa,Fanari', 'Argos,Arkadia,Vostitsa,Gastouni,Katw Axaia,Kalavruta,Karytaina,Katw Naxagies,Korinthos,Lexaina,Lipovitsi,Megara,Patra,Poros,Purgos,Salamina,Ydra'),
(18, 'Monemvasia', '', 20, 15, 10, 20, 15, 10, 0, 14, 1, 80, 0, 0, 0, '1.00', 177, -161, 0, 0, 2, 0, 8, 0, 'Mystras,Tripolitsa', 'Agios Petros,Karytaina,Koutsouk Mani,Mani,Leontari,Nhsi,Skala,Spetses,Ydra', 'Aigina,Androusa,Argos,Katw Axaia,Katw Naxagies,Korwni,Methwni,Emplakika,Navarino,Nauplio,Poros,Prastos,Fanari', 'Arkadia,Vostitsa,Gastouni,Kalavruta,Korinthos,Lexaina,Lipovitsi,Megara,Patra,Purgos,Salamina'),
(19, 'Prastos', '', 40, 10, 10, 40, 10, 10, 0, 18, 1, 90, 0, 0, 0, '1.00', 131, -21, 0, 0, 2, 0, 9, 0, 'Agios Petros,Skala,Mystras', 'Argos,Katw Naxagies,Salamina,Tripolitsa,Ydra', 'Androusa,Arkadia,Vostitsa,Korinthos,Korwni,Koutsouk Mani,Lexaina,Lipovitsi,Mani,Megara,Methwni,Emplakika,Monemvasia,Navarino,Nauplio,Nhsi,Patra,Poros,Spetses,Fanari', 'Aigina,Gastouni,Katw Axaia,Kalavruta,Karytaina,Leontari,Purgos'),
(20, 'Leontari', '', 35, 10, 0, 35, 10, 0, 0, 14, 1, 55, 0, 0, 0, '1.00', -53, 34, 0, 0, 2, 0, 10, 0, 'Arkadia,Koutsouk Mani,Mani,Emplakika,Mystras,Fanari', 'Agios Petros,Androusa,Argos,Vostitsa,Gastouni,Lipovitsi,Navarino,Nhsi,Skala,Tripolitsa', 'Katw Axaia,Kalavruta,Karytaina,Katw Naxagies,Korinthos,Korwni,Lexaina,Methwni,Monemvasia,Nauplio,Prastos', 'Aigina,Megara,Patra,Poros,Purgos,Salamina,Spetses,Ydra'),
(21, 'Tripolitsa', '', 80, 80, 40, 80, 80, 40, 0, 60, 1, 360, 0, 0, 0, '1.00', 79, 60, 0, 0, 3, 1, 1, 0, 'Agios Petros,Argos,Kalavruta,Karytaina,Korinthos,Lipovitsi,Monemvasia', 'Arkadia,Vostitsa,Gastouni,Katw Axaia,Koutsouk Mani,Leontari,Fanari', 'Aigina,Androusa,Katw Naxagies,Korwni,Lexaina,Mani,Megara,Methwni,Emplakika,Mystras,Navarino,Nauplio,Nhsi,Patra,Poros,Prastos,Purgos,Salamina,Skala,Spetses,Ydra', ''),
(22, 'Argos', '', 20, 15, 10, 20, 15, 10, 0, 14, 1, 80, 0, 0, 0, '1.00', 76, 139, 0, 0, 3, 0, 2, 0, 'Korinthos,Agios Petros,Nauplio,Tripolitsa', 'Aigina,Vostitsa,Kalavruta,Karytaina,Katw Naxagies,Leontari,Lipovitsi,Megara,Mystras,Poros,Prastos,Salamina,Spetses,Ydra', 'Androusa,Gastouni,Arkadia,Katw Axaia,Koutsouk Mani,Mani,Emplakika,Monemvasia,Fanari', 'Korwni,Lexaina,Methwni,Navarino,Nhsi,Patra,Purgos,Skala'),
(23, 'Lipovitsi', 'Kolokotrwnis', 40, 20, 20, 40, 20, 20, 0, 24, 1, 140, 0, 0, 0, '1.00', -42, 116, 0, 0, 3, 0, 3, 0, 'Gastouni,Kalavruta,Karytaina,Tripolitsa', 'Agios Petros,Aigina,Argos,Arkadia,Vostitsa,Katw Axaia,Korinthos,Leontari,Lexaina,Emplakika,Fanari', 'Androusa,Katw Naxagies,Koutsouk Mani,Mystras,Nauplio,Nhsi,Patra,Prastos,Purgos', 'Korwni,Mani,Megara,Methwni,Monemvasia,Navarino,Poros,Salamina,Skala,Spetses,Ydra'),
(24, 'Agios Petros', '', 30, 15, 0, 30, 15, 0, 0, 14, 1, 60, 0, 0, 0, '1.00', -72, 74, 0, 0, 3, 0, 4, 0, 'Tripolitsa,Argos,Mystras,Prastos,Spetses', 'Kalavruta,Karytaina,Korinthos,Koutsouk Mani,Leontari,Lipovitsi,Mani,Monemvasia,Nauplio,Skala', 'Aigina,Androusa,Arkadia,Vostitsa,Gastouni,Katw Axaia,Katw Naxagies,Megara,Emplakika,Nhsi,Poros,Ydra,Fanari', 'Korwni,Lexaina,Methwni,Navarino,Patra,Purgos,Salamina'),
(25, 'Karytaina', '', 40, 10, 10, 40, 10, 10, 0, 18, 1, 90, 0, 0, 0, '1.00', -10, 80, 0, 0, 3, 0, 5, 0, 'Gastouni,Leontari,Lipovitsi,Tripolitsa,Fanari', 'Agios Petros,Aigina,Androusa,Argos,Arkadia,Vostitsa,Katw Axaia,Kalavruta,Korinthos,Emplakika,Patra,Purgos,', 'Koutsouk Mani,Lexaina,Mystras,Nauplio,Nhsi,Spetses', 'Katw Naxagies,Korwni,Mani,Megara,Methwni,Monemvasia,Navarino,Poros,Prastos,Salamina,Skala,Ydra'),
(26, 'Spetses', 'Mpoumpoulina', 20, 30, 40, 20, 30, 40, 0, 27, 1, 200, 0, 0, 0, '1.00', 203, 23, 0, 0, 3, 0, 6, 0, 'Agios Petros,Katw Naxagies,Nauplio,Prastos,Ydra', 'Aigina,Argos,Korinthos,Megara,Monemvasia,Poros,Salamina', 'Vostitsa,Kalavruta,Karytaina,Koutsouk Mani,Mani,Emplakika,Mystras,Skala,Tripolitsa', 'Androusa,Arkadia,Gastouni,Katw Axaia,Korwni,Leontari,Lexaina,Lipovitsi,Methwni,Navarino,Nhsi,Patra,Purgos,Fanari'),
(27, 'Ydra', '', 0, 10, 35, 0, 10, 35, 0, 14, 1, 125, 0, 0, 0, '1.00', 296, 47, 0, 0, 3, 0, 7, 0, 'Katw Naxagies,Korinthos,Nauplio,Poros,Spetses', 'Aigina,Argos,Megara,Monemvasia,Prastos,Salamina', 'Agios Petros,Tripolitsa', 'Androusa,Arkadia,Vostitsa,Gastouni,Katw Axaia,Kalavruta,Karytaina,Korwni,Koutsouk Mani,Leontari,Lexaina,Lipovitsi,Mani,Methwni,Emplakika,Mystras,Navarino,Nhsi,Patra,Purgos,Skala,Fanari'),
(28, 'Poros', '', 20, 10, 0, 20, 10, 0, 0, 9, 1, 40, 0, 0, 0, '1.00', 300, 109, 0, 0, 3, 0, 8, 0, 'Aigina,Arkadia,Katw Axaia,Korinthos,Nauplio,Ydra', 'Argos,Gastouni,Katw Naxagies,Megara,Salamina,Spetses', 'Agios Petros,Monemvasia,Prastos,Tripolitsa', 'Androusa,Vostitsa,Kalavruta,Karytaina,Korwni,Koutsouk Mani,Leontari,Lexaina,Lipovitsi,Mani,Methwni,Emplakika,Mystras,Navarino,Nhsi,Patra,Purgos,Skala,Fanari'),
(29, 'Aigina', '', 20, 10, 0, 20, 10, 0, 0, 9, 1, 40, 0, 0, 0, '1.00', 304, 183, 0, 0, 3, 0, 9, 0, 'Poros,Korinthos,Megara,Salamina', 'Argos,Vostitsa,Katw Axaia,Kalavruta,Katw Naxagies,Nauplio,Spetses,Ydra', 'Agios Petros,Gastouni,Karytaina,Koutsouk Mani,Leontari,Lipovitsi,Mani,Emplakika,Monemvasia,Mystras,Nhsi,Patra,Prastos,Skala,Tripolitsa', 'Androusa,Arkadia,Korwni,Lexaina,Methwni,Navarino,Purgos,Fanari'),
(30, 'Salamina', '', 20, 10, 0, 20, 10, 0, 0, 9, 1, 40, 0, 0, 0, '1.00', 300, 244, 0, 0, 3, 0, 10, 0, 'Aigina,Gastouni,Korinthos,Megara', 'Argos,Natw Naxagies,Nauplio,Poros,Spetses,Ydra', 'Arkadia,Vostitsa,Tripolitsa,Fanari', 'Agios Petros,Androusa,Katw Axaia,Kalavruta,Karytaina,Korwni,Koutsouk Mani,Leontari,Lexaina,Lipovitsi,Mani,Methwni,Emplakika,Monemvasia,Mystras,Navarino,Nhsi,Patra,Prastos,Purgos,Skala'),
(31, 'Megara', '', 30, 0, 0, 30, 0, 0, 0, 9, 1, 30, 0, 0, 0, '1.00', 262, 266, 0, 0, 4, 1, 1, 0, 'Aigina,Korinthos,Salamina', 'Argos,Vostitsa,Kalavruta,Nauplio,Poros,Spetses,Ydra', 'Agios Petros,Katw Naxagies,Emplakika,Mystras,Nhsi,Patra,Prastos,Tripolitsa', 'Androusa,Arkadia,Gastouni,Katw Axaia,Karytaina,Korwni,Koutsouk Mani,Leontari,Lexaina,Lipovitsi,Mani,Methwni,Monemvasia,Navarino,Purgos,Skala,Fanari'),
(32, 'Korinthos', '', 20, 15, 10, 20, 15, 10, 0, 14, 1, 80, 0, 0, 0, '1.00', 165, 240, 0, 0, 4, 0, 2, 0, 'Argos,Vostitsa,Kalavruta,Megara,Nauplio,Poros,Salamina,Tripolitsa,Ydra', 'Agios Petros,Katw Axaia,Karytaina,Katw Naxagies,Lipovitsi,Patra,Spetses', 'Androusa,Arkadia,Gastouni,Koutsouk Mani,Leontari,Lexaina,Emplakika,Mystras,Nhsi,Prastos,Purgos,Fanari', 'Aigina,Korwni,Mani,Methwni,Monemvasia,Navarino,Skala'),
(33, 'Katw Naxagies', '', 20, 15, 10, 20, 15, 10, 0, 14, 0, 80, 0, 0, 0, '1.00', 223, 52, 0, 0, 0, 0, 0, 0, 'Aigina,Argos,Spetses,Nauplio,Ydra', 'Korinthos,Mystras,Poros,Prastos,Salamina', 'Agios Petros,Koutsouk Mani,Leontari,Lipovitsi,Megara,Emplakika,Monemvasia,Nhsi,Tripolitsa', 'Androusa,Arkadia,Vostitsa,Gastouni,Katw Axaia,Kalavruta,Karytaina,Korwni,Lexaina,Mani,Methwni,Navarino,Patra,Purgos,Skala,Fanari'),
(34, 'Nauplio', '', 20, 40, 30, 20, 40, 30, 0, 27, 0, 190, 0, 0, 0, '1.00', 152, 120, 0, 0, 0, 0, 0, 0, 'Argos,Katw Naxagies,Korinthos,Poros,Spetses,Ydra', 'Agios Petros,Aigina,Kalavruta,Megara,Salamina', 'Androusa,Arkadia,Vostitsa,Gastouni,Korwni,Koutsouk Mani,Lexaina,Lipovitsi,Mani,Methwni,Emplakika,Monemvasia,Mystras,Navarino,Nhsi,Prastos,Skala,Tripolitsa,Fanari', 'Katw Axaia,Karytaina,Leontari,Patra,Purgos'),
(35, 'Mystras', '', 50, 50, 20, 50, 50, 20, 0, 36, 0, 210, 0, 0, 0, '1.00', 34, -49, 0, 0, 0, 0, 0, 0, 'Agios Petros,Koutsouk Mani,Mani,Monemvasia,Prastos,Skala', 'Androusa,Argos,Kalavruta,Katw Naxagies,Korwni,Methwni,Emplakika,Nhsi', 'Aigina,Arkadia,Vostitsa,Gastouni,Korinthos,Leontari,Lexaina,Lipovitsi,Megara,Navarino,Nauplio,Purgos,Spetses,Tripolitsa,Fanari', 'Katw Axaia,Karytaina,Patra,Poros,Salamina,Ydra'),
(36, 'Androusa', '', 40, 30, 20, 40, 30, 20, 0, 27, 0, 160, 0, 0, 0, '1.00', -125, -86, 0, 0, 0, 0, 0, 0, 'Arkadia,Korwni,Emplakika,Methwni,Navarino,Koutsouk Mani,Nhsi', 'Karytaina,Leontari,Mani,Mystras,Fanari', 'Agios Petros,Argos,Gastouni,Vostitsa,Katw Axaia,Kalavruta,Korinthos,Lexaina,Lipovitsi,Monemvasia,Nauplio,Patra,Prastos,Purgos,Skala,Tripolitsa', 'Aigina,Katw Naxagies,Megara,Poros,Salamina,Spetses,Ydra');

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
-- Table structure for table `support`
--

CREATE TABLE IF NOT EXISTS `support` (
  `id` int(30) NOT NULL,
  `requestor` varchar(30) NOT NULL,
  `supporter` varchar(30) NOT NULL,
  `req_region` varchar(30) NOT NULL,
  `sup_region` varchar(30) NOT NULL,
  `turn` int(30) NOT NULL,
  `army` int(30) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=latin1;

-- --------------------------------------------------------

--
-- Table structure for table `support_check`
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
-- Table structure for table `treaties`
--

CREATE TABLE IF NOT EXISTS `treaties` (
  `player_a` varchar(30) NOT NULL,
  `player_b` varchar(30) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=latin1;

/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
