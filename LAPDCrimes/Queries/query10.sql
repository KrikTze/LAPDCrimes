-- this is wrong
-- group by and order
WITH grouped as 
(
SELECT ar.area_name as area_name, cr.area_code as area_code, date_occ
FROM "CrimesSchema".crimes_commited as cr_c
INNER JOIN "CrimesSchema".crimes as cr
ON cr.drno = cr_c.drno
INNER JOIN "CrimesSchema".areas as ar
ON cr.area_code = ar.area_code
WHERE crime_code = '121'
GROUP BY cr.area_code, ar.area_name ,date_occ
ORDER BY cr.area_code, date_occ
),
date_buddies as(
SELECT gr1.area_name as area_name, gr1.area_code as area_code, gr1.date_occ as date1, gr2.date_occ as date2, gr1.date_occ - gr2.date_occ as datedif
FROM grouped as gr1
INNER JOIN grouped as gr2
ON gr1.area_name = gr2.area_name AND gr1.area_code = gr2.area_code 
WHERE gr1.date_occ != gr2.date_occ AND gr1.date_occ - gr2.date_occ > 0
),
min_difs as (
SELECT area_code,date1,MIN(datedif) as datedif
FROM date_buddies as dbuddies1
GROUP BY area_code,date1
), 
max_per_date as (
SELECT area_name, dbuddies.area_code , dbuddies.date1 , date2, mdifs.datedif as datedif
FROM date_buddies as dbuddies
INNER JOIN min_difs as mdifs
ON dbuddies.area_code = mdifs.area_code AND dbuddies.date1 = mdifs.date1 AND dbuddies.datedif = mdifs.datedif
ORDER BY dbuddies.area_code,datedif DESC
),
max_tables as (
SELECT area_name, MAX(datedif) as datedif
FROM max_per_date
GROUP BY area_name
)
SELECT mt.area_name, md.area_code , md.date1 , date2, md.datedif as time_range
FROM max_per_date as md
INNER JOIN max_tables as mt 
ON md.area_name = mt.area_name and md.datedif = mt.datedif
ORDER BY md.datedif DESC
LIMIT 1

