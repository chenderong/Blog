/*
Navicat MySQL Data Transfer

Source Server         : dcoker_mysql01
Source Server Version : 80018
Source Host           : localhost:3305
Source Database       : dncblogs

Target Server Type    : MYSQL
Target Server Version : 80018
File Encoding         : 65001

Date: 2019-11-03 15:28:07
*/

SET FOREIGN_KEY_CHECKS=0;

-- ----------------------------
-- Table structure for blog
-- ----------------------------
DROP TABLE IF EXISTS `blog`;
CREATE TABLE `blog` (
  `BlogId` bigint(11) NOT NULL AUTO_INCREMENT,
  `CategoryID` int(11) NOT NULL,
  `Title` varchar(500) DEFAULT NULL,
  `Body` longtext,
  `KeyWord` varchar(200) DEFAULT NULL,
  `Description` varchar(500) DEFAULT NULL,
  `Remark` varchar(500) DEFAULT NULL,
  `CreateDate` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  `UpdateDate` datetime DEFAULT NULL ON UPDATE CURRENT_TIMESTAMP,
  `Sort` int(11) DEFAULT '0',
  `UserId` int(11) DEFAULT '0',
  `VisitCount` int(11) DEFAULT '0',
  `CommentCount` int(11) DEFAULT '0',
  `IsEssence` bit(1) DEFAULT b'0',
  `isHomePage` bit(1) DEFAULT NULL,
  `IsDelete` bit(1) NOT NULL DEFAULT b'0',
  PRIMARY KEY (`BlogId`),
  KEY `CategoryID` (`CategoryID`),
  KEY `CreateDate` (`CreateDate`),
  KEY `UpdateDate` (`UpdateDate`),
  KEY `UserId` (`UserId`),
  KEY `Sort` (`Sort`),
  KEY `IsDelete` (`IsDelete`),
  KEY `IsEssence` (`IsEssence`)
) ENGINE=InnoDB AUTO_INCREMENT=19 DEFAULT CHARSET=utf8;

-- ----------------------------
-- Table structure for blogcomment
-- ----------------------------
DROP TABLE IF EXISTS `blogcomment`;
CREATE TABLE `blogcomment` (
  `BCId` bigint(20) NOT NULL AUTO_INCREMENT,
  `BlogId` bigint(20) DEFAULT NULL,
  `ReferenceId` bigint(20) DEFAULT NULL,
  `PostId` int(11) DEFAULT '0',
  `PostName` varchar(255) DEFAULT NULL,
  `Content` varchar(1000) DEFAULT NULL,
  `PostDate` datetime DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  `IsDelete` bit(1) DEFAULT b'0',
  PRIMARY KEY (`BCId`),
  KEY `BCId` (`BCId`),
  KEY `BlogId` (`BlogId`),
  KEY `PostDate` (`PostDate`),
  KEY `IsDelete` (`IsDelete`)
) ENGINE=InnoDB AUTO_INCREMENT=146 DEFAULT CHARSET=utf8;

-- ----------------------------
-- Table structure for category
-- ----------------------------
DROP TABLE IF EXISTS `category`;
CREATE TABLE `category` (
  `CategoryId` int(11) NOT NULL AUTO_INCREMENT,
  `CategoryName` varchar(100) DEFAULT NULL,
  `ParentId` int(11) DEFAULT '0',
  `CreateDate` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  `UserId` int(11) DEFAULT NULL,
  `Sort` int(11) DEFAULT '0',
  `IsDelete` bit(1) DEFAULT b'0',
  PRIMARY KEY (`CategoryId`),
  KEY `CategoryId` (`CategoryId`),
  KEY `ParentId` (`ParentId`),
  KEY `Sort` (`Sort`),
  KEY `IsDelete` (`IsDelete`)
) ENGINE=InnoDB AUTO_INCREMENT=37 DEFAULT CHARSET=utf8;

-- ----------------------------
-- Table structure for news
-- ----------------------------
DROP TABLE IF EXISTS `news`;
CREATE TABLE `news` (
  `NewsId` bigint(20) NOT NULL AUTO_INCREMENT,
  `Title` varchar(500) DEFAULT NULL,
  `Body` longtext,
  `KeyWord` varchar(255) DEFAULT NULL,
  `Description` varchar(255) DEFAULT NULL,
  `VisitCount` int(11) DEFAULT NULL,
  `CommentCount` int(11) DEFAULT NULL,
  `CreateDate` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  `OriginalUrl` varchar(255) DEFAULT NULL,
  `Sort` int(11) DEFAULT '0',
  `IsDelete` bit(1) NOT NULL DEFAULT b'0',
  PRIMARY KEY (`NewsId`),
  KEY `NewsId` (`NewsId`),
  KEY `CreateDate` (`CreateDate`),
  KEY `Sort` (`Sort`),
  KEY `IsDelete` (`IsDelete`),
  KEY `CreateDateSort` (`CreateDate`,`Sort`)
) ENGINE=InnoDB AUTO_INCREMENT=6 DEFAULT CHARSET=utf8;

-- ----------------------------
-- Table structure for newscomment
-- ----------------------------
DROP TABLE IF EXISTS `newscomment`;
CREATE TABLE `newscomment` (
  `Nid` bigint(20) NOT NULL AUTO_INCREMENT,
  `NewsId` bigint(20) DEFAULT NULL,
  `ReferenceId` bigint(20) DEFAULT NULL,
  `PostId` int(11) DEFAULT NULL,
  `PostName` varchar(255) DEFAULT NULL,
  `Content` varchar(1000) DEFAULT NULL,
  `PostDate` datetime DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  `IsDelete` bit(1) DEFAULT b'0',
  PRIMARY KEY (`Nid`),
  KEY `Nid` (`Nid`),
  KEY `NewsId` (`NewsId`),
  KEY `ReferenceId` (`ReferenceId`),
  KEY `PostId` (`PostId`),
  KEY `PostDate` (`PostDate`),
  KEY `IsDelete` (`IsDelete`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- ----------------------------
-- Table structure for oauthuser
-- ----------------------------
DROP TABLE IF EXISTS `oauthuser`;
CREATE TABLE `oauthuser` (
  `UserId` bigint(20) NOT NULL,
  `GitHub_login` varchar(200) CHARACTER SET utf8 COLLATE utf8_general_ci DEFAULT NULL,
  `GitHub_id` bigint(20) DEFAULT NULL,
  PRIMARY KEY (`UserId`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- ----------------------------
-- Table structure for opensource
-- ----------------------------
DROP TABLE IF EXISTS `opensource`;
CREATE TABLE `opensource` (
  `OpenSourceID` int(11) NOT NULL AUTO_INCREMENT,
  `OpenSourceType` varchar(500) DEFAULT NULL,
  `OpenSourceTitle` varchar(1000) DEFAULT NULL,
  `OpenSourceDescribe` varchar(3000) DEFAULT NULL,
  `OpenSourceContent` longtext,
  `CreateDate` datetime DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  `HeatType` int(11) DEFAULT '0' COMMENT '0 普通 1 热门  2 编辑推荐',
  `Sort` int(11) DEFAULT '0',
  `IsDelete` bit(1) DEFAULT b'0',
  PRIMARY KEY (`OpenSourceID`),
  KEY `CreateDate` (`CreateDate`),
  KEY `Sort` (`Sort`),
  KEY `IsDelete` (`IsDelete`),
  KEY `HeatType` (`HeatType`)
) ENGINE=InnoDB AUTO_INCREMENT=10 DEFAULT CHARSET=utf8;

-- ----------------------------
-- Table structure for suggest
-- ----------------------------
DROP TABLE IF EXISTS `suggest`;
CREATE TABLE `suggest` (
  `SuggestID` int(11) NOT NULL AUTO_INCREMENT,
  `SuggestContact` varchar(255) DEFAULT NULL,
  `SuggestContent` varchar(2000) DEFAULT NULL,
  `CreateDate` datetime DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  PRIMARY KEY (`SuggestID`),
  KEY `CreateDate` (`CreateDate`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- ----------------------------
-- Table structure for user
-- ----------------------------
DROP TABLE IF EXISTS `user`;
CREATE TABLE `user` (
  `UserId` bigint(11) NOT NULL AUTO_INCREMENT,
  `LoginName` varchar(50) NOT NULL,
  `UserName` varchar(50) DEFAULT NULL,
  `UserHeadImaUrl` varchar(255) DEFAULT NULL,
  `Password` varchar(50) DEFAULT NULL,
  `BlogTitle` varchar(255) DEFAULT NULL,
  `BlogName` varchar(255) DEFAULT NULL,
  `BlogDesc` varchar(500) DEFAULT NULL,
  `BlogNotice` varchar(300) DEFAULT NULL,
  `CreateDate` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  `Status` int(11) NOT NULL DEFAULT '0',
  `IsAdmin` bit(1) NOT NULL DEFAULT b'0',
  `IsDelete` bit(1) NOT NULL DEFAULT b'0',
  PRIMARY KEY (`UserId`),
  KEY `UserId` (`UserId`),
  KEY `LoginName` (`LoginName`),
  KEY `IsDelete` (`IsDelete`)
) ENGINE=InnoDB AUTO_INCREMENT=11 DEFAULT CHARSET=utf8;
