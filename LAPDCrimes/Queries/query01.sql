SELECT crime_code, COUNT(*) as counter
FROM "CrimesSchema".crimes_commited as jun
INNER JOIN "CrimesSchema".crimes as main
ON jun.drno = main.drno
WHERE CAST(main.time_occ AS INT) > 1000 and CAST(main.time_occ AS INT) < 1100
GROUP BY crime_code
ORDER BY counter DESC;
