CREATE DATABASE IF NOT EXISTS `library-mnaagement-system` CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;

USE `library-mnaagement-system`;

-- 管理员表
CREATE TABLE IF NOT EXISTS `Admins` (
    `Account` VARCHAR(50) PRIMARY KEY,
    `Name` VARCHAR(50) NOT NULL,
    `Password` VARCHAR(50) NOT NULL
) ENGINE=InnoDB;

-- 读者表 (用户)
CREATE TABLE IF NOT EXISTS `Readers` (
    `StudentId` VARCHAR(50) PRIMARY KEY,
    `Name` VARCHAR(50) NOT NULL,
    `Department` VARCHAR(100),
    `Phone` VARCHAR(20),
    `Password` VARCHAR(50) NOT NULL
) ENGINE=InnoDB;

-- 图书表
CREATE TABLE IF NOT EXISTS `Books` (
    `Id` INT AUTO_INCREMENT PRIMARY KEY,
    `Title` VARCHAR(255) NOT NULL,
    `Author` VARCHAR(100),
    `ISBN` VARCHAR(50),
    `Category` VARCHAR(50),
    `Location` VARCHAR(100),
    `TotalStock` INT DEFAULT 0,
    `AvailableStock` INT DEFAULT 0,
    `BorrowCount` INT DEFAULT 0
) ENGINE=InnoDB;

-- 借阅记录表
CREATE TABLE IF NOT EXISTS `BorrowRecords` (
    `RecordId` INT AUTO_INCREMENT PRIMARY KEY,
    `BookId` INT NOT NULL,
    `BookTitle` VARCHAR(255),
    `ReaderStudentId` VARCHAR(50) NOT NULL,
    `ReaderName` VARCHAR(50),
    `BorrowDate` DATETIME NOT NULL,
    `DueDate` DATETIME NOT NULL,
    `IsReturned` BOOLEAN DEFAULT FALSE,
    `ReturnDateText` VARCHAR(50),
    FOREIGN KEY (`BookId`) REFERENCES `Books`(`Id`) ON DELETE CASCADE,
    FOREIGN KEY (`ReaderStudentId`) REFERENCES `Readers`(`StudentId`) ON DELETE CASCADE
) ENGINE=InnoDB;

-- 插入初始数据 (根据 LibraryRepository.cs 中的种子数据)
INSERT IGNORE INTO `Admins` (`Account`, `Name`, `Password`) VALUES
('admin', '系统管理员', '123456'),
('manager01', '馆藏管理员', '123456');

INSERT IGNORE INTO `Readers` (`StudentId`, `Name`, `Department`, `Phone`, `Password`) VALUES
('2023001', '张三', '计算机学院', '13800000001', '123456'),
('2023002', '李四', '软件学院', '13800000002', '123456'),
('2023003', '王五', '信息学院', '13800000003', '123456');

INSERT IGNORE INTO `Books` (`Id`, `Title`, `Author`, `ISBN`, `Category`, `Location`, `TotalStock`, `AvailableStock`, `BorrowCount`) VALUES
(1, 'C# 程序设计', '王老师', '978730000001', '程序设计', 'A-101', 8, 6, 2),
(2, '数据库系统概论', '陈老师', '978730000002', '数据库', 'A-102', 6, 5, 1),
(3, '数据结构', '严老师', '978730000003', '计算机基础', 'B-201', 10, 8, 2),
(4, '软件工程导论', '周老师', '978730000004', '软件工程', 'B-202', 5, 5, 0),
(5, '操作系统原理', '刘老师', '978730000005', '系统原理', 'C-105', 7, 7, 0);

INSERT IGNORE INTO `BorrowRecords` (`RecordId`, `BookId`, `BookTitle`, `ReaderStudentId`, `ReaderName`, `BorrowDate`, `DueDate`, `IsReturned`, `ReturnDateText`) VALUES
(1, 1, 'C# 程序设计', '2023001', '张三', DATE_SUB(CURDATE(), INTERVAL 5 DAY), DATE_ADD(CURDATE(), INTERVAL 25 DAY), FALSE, ''),
(2, 3, '数据结构', '2023002', '李四', DATE_SUB(CURDATE(), INTERVAL 40 DAY), DATE_SUB(CURDATE(), INTERVAL 10 DAY), FALSE, ''),
(3, 2, '数据库系统概论', '2023003', '王五', DATE_SUB(CURDATE(), INTERVAL 25 DAY), DATE_SUB(CURDATE(), INTERVAL 2 DAY), TRUE, DATE_SUB(CURDATE(), INTERVAL 1 DAY));
