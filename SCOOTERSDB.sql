CREATE DATABASE SCOOTERS2;

USE SCOOTERS2;
--select * from USUARIO
CREATE TABLE USUARIO (
    USU_ID INT PRIMARY KEY IDENTITY(1,1),
    USU_NOMBRE VARCHAR(50),
    USU_CONTRASENA VARCHAR(10)
);
SELECT * FROM PATIN
CREATE TABLE PATIN (
    PAT_ID INT IDENTITY(1,1) PRIMARY KEY,     -- ID único del scooter
    PAT_NOMBRE VARCHAR(20) NOT NULL, 
	PAT_IMAGEN VARCHAR (255),         
    PAT_IS_AVAILABLE BIT NOT NULL DEFAULT 1   -- Indica si el scooter está disponible (1) o en uso (0)
);


CREATE TABLE MINUTOS (
    MIN_ID TINYINT IDENTITY(1,1) PRIMARY KEY,                  -- ID único del tipo de tiempo
    MIN_TIEMPO INT NOT NULL,                     -- Duración en minutos
    MIN_COSTO FLOAT NOT NULL                     -- Costo asociado para el tiempo especificado
);


CREATE TABLE ALQUILER (
    ALQ_ID INT IDENTITY(1,1) PRIMARY KEY,            -- ID único del alquiler
    ALQ_PAT_ID INT NOT NULL,                         -- ID del scooter alquilado
    ALQ_CLI_TELEFONO VARCHAR(15) NOT NULL,           -- Teléfono del cliente
    ALQ_CLI_CREDENCIAL VARBINARY(MAX) NOT NULL,      -- Imagen de la credencial del cliente en binario
    ALQ_MIN_ID TINYINT,                              -- ID del tipo de tiempo (relación con MINUTOS)
    ALQ_COST FLOAT NOT NULL,                         -- Costo total del alquiler
    ALQ_REMAINING_TIME INT NOT NULL,                 -- Tiempo restante en segundos
    ALQ_TIME_UP DATETIME NOT NULL,                   -- Fecha y hora en que el tiempo se acaba
    ALQ_IS_RUNNING BIT NOT NULL,                     -- Indica si el alquiler está activo o no,
    FOREIGN KEY (ALQ_PAT_ID) REFERENCES PATIN(PAT_ID),
    FOREIGN KEY (ALQ_MIN_ID) REFERENCES MINUTOS(MIN_ID)
);



INSERT INTO MINUTOS (MIN_TIEMPO, MIN_COSTO) VALUES (15, 50.00);
INSERT INTO MINUTOS (MIN_TIEMPO, MIN_COSTO) VALUES (30, 100.00);
INSERT INTO MINUTOS (MIN_TIEMPO, MIN_COSTO) VALUES (45, 150.00);
INSERT INTO MINUTOS (MIN_TIEMPO, MIN_COSTO) VALUES (60, 200.00);

SELECT 
     p.PAT_NOMBRE AS NombrePatin, 
     p.PAT_IMAGEN AS ImagenPatin, 
     a.ALQ_COST AS CostoAlquiler, 
     a.ALQ_REMAINING_TIME AS TiempoRestante
FROM 
     ALQUILER a
INNER JOIN 
     PATIN p ON a.ALQ_PAT_ID = p.PAT_ID
WHERE 
     a.ALQ_IS_RUNNING = 1;


	 DELETE FROM patin;



	 SELECT ALQ_ID, ALQ_PAT_ID, ALQ_CLI_TELEFONO, ALQ_CLI_CREDENCIAL, ALQ_MIN_ID, ALQ_COST, ALQ_REMAINING_TIME, ALQ_TIME_UP, ALQ_IS_RUNNING FROM ALQUILER


	 DELETE FROM ALQUILER;













--RESTORE DATABASE SCOOTERS WITH RECOVERY;
--SELECT name, state_desc FROM sys.databases WHERE name = 'SCOOTERS';
--ALTER DATABASE SCOOTERS SET EMERGENCY;
--ALTER DATABASE SCOOTERS SET SINGLE_USER;
--DBCC CHECKDB (SCOOTERS, REPAIR_ALLOW_DATA_LOSS);
--ALTER DATABASE SCOOTERS SET MULTI_USER;