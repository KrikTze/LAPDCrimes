# LAPDCrimes

Project01 - M149

Name: Krikor 
Surname: Tzevachirian
AM: 7115112400005

## Introduction: 
Most of the objectives required have been completed.
The webapp supports authentication and authorization.
10/12 queries are successfully executed.


## CrimesDB Schema:
![Untitled](https://github.com/user-attachments/assets/7265a9bf-c37f-4ab8-8f76-2cdcbc928aba)
The csv has been broken down to the following 11 tables 

CREATE TABLE IF NOT EXISTS "CrimesSchema".crime_premis
(
    premis_code character(3) NOT NULL,
    premis_desc character varying(255) NOT NULL,
    CONSTRAINT crime_premis_pkey PRIMARY KEY (premis_code)
);

CREATE TABLE IF NOT EXISTS "CrimesSchema".crimes
(
    drno character(9) NOT NULL,
    date_rptd date NOT NULL,
    date_occ date NOT NULL,
    time_occ character(4),
    area_code character(2) ,
    reporting_distrc character(4),
    loc_id uuid NOT NULL,
    wpn_code character(3) ",
    inv_status character(2) NOT NULL DEFAULT 'IC'::bpchar,
    mocodes character varying(255) ,
    premis_code character(3) ,
    CONSTRAINT crimes_pkey PRIMARY KEY (drno)
);

CREATE TABLE IF NOT EXISTS "CrimesSchema".areas
(
    area_code character(2) NOT NULL,
    area_name character varying(255) NOT NULL,
    CONSTRAINT areas_pkey PRIMARY KEY (area_code)
);

CREATE TABLE IF NOT EXISTS "CrimesSchema".crimes_inv_status
(
    status_code character(2) NOT NULL,
    status_desc character varying(255) NOT NULL,
    CONSTRAINT crimes_inv_status_pkey PRIMARY KEY (status_code),
    CONSTRAINT crimes_inv_status_status_desc_key UNIQUE (status_desc)
);

CREATE TABLE IF NOT EXISTS "CrimesSchema".location
(
    loc_id uuid NOT NULL DEFAULT gen_random_uuid(),
    address character varying(255),
    cross_street character varying(255),
    loc_point geography,
    CONSTRAINT location_pkey PRIMARY KEY (loc_id)
);

CREATE TABLE IF NOT EXISTS "CrimesSchema".report_district
(
    district_code character(4)  NOT NULL,
    CONSTRAINT report_district_pkey PRIMARY KEY (district_code)
);

CREATE TABLE IF NOT EXISTS "CrimesSchema".crime_weapons
(
    wpn_code character(3) NOT NULL,
    wpn_desc character varying(255) NOT NULL,
    CONSTRAINT crime_weapons_pkey PRIMARY KEY (wpn_code),
    CONSTRAINT crime_weapons_wpn_desc_key UNIQUE (wpn_desc)
);

CREATE TABLE IF NOT EXISTS "CrimesSchema".crimes_commited
(
    drno character(9) NOT NULL,
    crime_code character(3) NOT NULL,
    ismaincrime boolean NOT NULL DEFAULT false,
    CONSTRAINT crimes_commited_pkey PRIMARY KEY (drno, crime_code)
);

CREATE TABLE IF NOT EXISTS "CrimesSchema".crime_codes
(
    crm_code character(3) NOT NULL,
    crm_descr character varying(255) NOT NULL,
    CONSTRAINT crime_codes_pkey PRIMARY KEY (crm_code)
);

CREATE TABLE IF NOT EXISTS "CrimesSchema".crimes_victims
(
    drno character(9) NOT NULL,
    victim_id uuid NOT NULL,
    CONSTRAINT crimes_victims_pkey PRIMARY KEY (drno, victim_id)
);

CREATE TABLE IF NOT EXISTS "CrimesSchema".victims
(
    victim_id uuid NOT NULL DEFAULT gen_random_uuid(),
    victim_age integer NOT NULL DEFAULT 0,
    victim_descent character(1) DEFAULT 'X'::bpchar,
    victim_sex character(1) DEFAULT 'X'::bpchar,
    CONSTRAINT victims_pkey PRIMARY KEY (victim_id)
);

ALTER TABLE IF EXISTS "CrimesSchema".crimes
    ADD CONSTRAINT fk_area_code FOREIGN KEY (area_code)
    REFERENCES "CrimesSchema".areas (area_code) MATCH SIMPLE
    ON UPDATE NO ACTION
    ON DELETE NO ACTION;


ALTER TABLE IF EXISTS "CrimesSchema".crimes
    ADD CONSTRAINT fk_inv_status FOREIGN KEY (inv_status)
    REFERENCES "CrimesSchema".crimes_inv_status (status_code) MATCH SIMPLE
    ON UPDATE NO ACTION
    ON DELETE NO ACTION;


ALTER TABLE IF EXISTS "CrimesSchema".crimes
    ADD CONSTRAINT fk_loc_id FOREIGN KEY (loc_id)
    REFERENCES "CrimesSchema".location (loc_id) MATCH SIMPLE
    ON UPDATE NO ACTION
    ON DELETE NO ACTION;


ALTER TABLE IF EXISTS "CrimesSchema".crimes
    ADD CONSTRAINT fk_premis_code FOREIGN KEY (premis_code)
    REFERENCES "CrimesSchema".crime_premis (premis_code) MATCH SIMPLE
    ON UPDATE NO ACTION
    ON DELETE NO ACTION;


ALTER TABLE IF EXISTS "CrimesSchema".crimes
    ADD CONSTRAINT fk_reporting_distrc FOREIGN KEY (reporting_distrc)
    REFERENCES "CrimesSchema".report_district (district_code) MATCH SIMPLE
    ON UPDATE NO ACTION
    ON DELETE NO ACTION;


ALTER TABLE IF EXISTS "CrimesSchema".crimes
    ADD CONSTRAINT fk_wpn_code FOREIGN KEY (wpn_code)
    REFERENCES "CrimesSchema".crime_weapons (wpn_code) MATCH SIMPLE
    ON UPDATE NO ACTION
    ON DELETE NO ACTION;


ALTER TABLE IF EXISTS "CrimesSchema".crimes_commited
    ADD CONSTRAINT crimes_commited_crime_code_fkey FOREIGN KEY (crime_code)
    REFERENCES "CrimesSchema".crime_codes (crm_code) MATCH SIMPLE
    ON UPDATE NO ACTION
    ON DELETE NO ACTION;


ALTER TABLE IF EXISTS "CrimesSchema".crimes_commited
    ADD CONSTRAINT crimes_commited_drno_fkey FOREIGN KEY (drno)
    REFERENCES "CrimesSchema".crimes (drno) MATCH SIMPLE
    ON UPDATE NO ACTION
    ON DELETE CASCADE;


ALTER TABLE IF EXISTS "CrimesSchema".crimes_victims
    ADD CONSTRAINT crimes_victims_drno_fkey FOREIGN KEY (drno)
    REFERENCES "CrimesSchema".crimes (drno) MATCH SIMPLE
    ON UPDATE NO ACTION
    ON DELETE CASCADE;


ALTER TABLE IF EXISTS "CrimesSchema".crimes_victims
    ADD CONSTRAINT crimes_victims_victim_id_fkey FOREIGN KEY (victim_id)
    REFERENCES "CrimesSchema".victims (victim_id) MATCH SIMPLE
    ON UPDATE NO ACTION
    ON DELETE NO ACTION;

    
## Design Choices:
1) During data insertion, I avoided dropping records due to the nature of the information.
I would rather have a record with some unreported information that drop the row.

2) While designing the database, I tried to break apart the csv to create a normalized schema with
little to no duplication. Eventually, arriving at the current Schema, which focuses around the main tables
crimes, location, crimes_commited, while the others act as a supporting role.

3) The location table is not fully normalized. One large address for example can have multiple lat-lon combos.
However, I decided against breaking it further apart since: i) the duplication was relatively small in the db (about 1.5 times).
ii) Conceptually these values belong together. iii) it makes querying easier.

4) A junction table is created between crime_codes and drnos for normalization and real-world application purposes.
To keep track of the main crime, a column with a boolean value is added.

5) The same reasoning was followed with victims, because in real life one crime can have multiple victims.

6) Indexes were added on the following columns ( apart from the primary keys ):
i) crimes.date_occ, ii) crimes.date_rptd, iii) location.loc_point
The first two are used extensively in the queries, as for the last I believe that
any type of meaningful extension will require some sort of geolocation querying.

7) To insert the data, I used a python script to first structure it and then insert it
using batches of 1000 - 10000 ( depending on the table ) for faster insertion

8) For the website, my framework of choice is .net8.0.
The entityframework package supports complex relationships between models and
the usage of linq and other techniques to query the data. However, since I had already
written the queries in raw sql, I decided to use the already completed queries instead of
creating and mapping the relationships between the tables.
For authentication-authorization the identityframework package is used.

## Use Case:
After launching the site, the user has to register in order to be able to query the database.
While logged in, the user has access to a list of predefined sql queries to parameterize.
After selecting a query and filling the required information. An Ajax request is executed and 
the results of the query are fetched back at the user.

## Additional Notes:
- 10/12 queries can be found in the sql scripts folder. However only the first 5 are available in the website
This is temporary and will change soon.
- The styling of the website will change as well.


