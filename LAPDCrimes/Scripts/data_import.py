import pandas as pd
import psycopg2 as psy
import uuid
from psycopg2.extras import execute_batch

def dropDuplicates(df,columnName):
    df = df.drop_duplicates(columnName)
    df = df.dropna(subset=columnName)
    return df

def keepUniqueCdDescr(df,columnCode,columnDescr):
    uniqueDf = df[[columnCode,columnDescr]].copy()
    uniqueDf = dropDuplicates(uniqueDf,columnCode)
    uniqueValuesList = uniqueDf.values.tolist()
    return uniqueValuesList

def addUuid(df,idName):
    df[idName] = [str(uuid.uuid4()) for _ in range(len(df))]
    return df


def executeTheQuery(cur,table_name, data, columns, page_size=1000):
    # Construct the SQL query dynamically
    columns_str = ', '.join(columns)
    
    placeholders = ', '.join(['%s'] * len(columns))
    query = f'INSERT INTO {table_name} ({columns_str}) VALUES ({placeholders})'
    # Execute the batch insertion
    print(query)
    execute_batch(cur, query, data, page_size)

def insertLocData(cur, data, page_size=1000):
    insert_query = """
    INSERT INTO "CrimesSchema".location (address, cross_street, loc_point, loc_id)
    VALUES (%s, %s, ST_SetSRID(ST_MakePoint(%s, %s), 4326), %s);
    """
    execute_batch(cur,insert_query,data,page_size)
    return

def nameTable(tableName):
    return f'"CrimesSchema".{tableName}'


def victimChecker(df, columnName):
    result = df[df[columnName].astype(str).str.len() > 1]
    print(result)

def createDefaults(df,defaultsDict,columns):
    for column in columns:
        if column in defaultsDict.keys():
            df[column] = df[column].apply(lambda x: defaultsDict[column] if pd.isnull(x) else x)
    return df


def findIds(table,newColumnName,all_data,data,columnsToCheck):
    merged_table = table.merge(all_data, on='DR_NO', how='left')

    # Step 2: Merge the result with 'data' on 'columnsToCheck' to get 'newColumnName'.
    merged_result = merged_table.merge(
        data[columnsToCheck + [newColumnName]],
        on=columnsToCheck,
        how='left'
    )

    # Step 3: Add the 'newColumnName' to the original 'table'.
    table[newColumnName] = merged_result[newColumnName]
    return table

def findVictms(crime_data,victim_data,alldata,columnsToCheck):
    table = pd.DataFrame()
    table["DR_NO"] = crime_data["DR_NO"]

    # Step 2: Merge the result with 'data' on 'columnsToCheck' to get 'newColumnName'.
    merged_result = all_data.merge(
        victim_data[columnsToCheck + ["victim_id"]],
        on=columnsToCheck,
        how='left'
    )
    # Step 3: Add the 'newColumnName' to the original 'table'.
    table["victim_id"] = merged_result["victim_id"]
    return table

default_values_dict = {
    "Vict Sex": "X",
    "Vict Descent": "X",
    "Vict Age":"0",
    "Status":"IC",
    "Cross Street":"",
    "LOCATION": "Unreported",
    "Cross Street": "Unreported",
    "Weapon Used Cd":None,
    "Premis Cd":None,
    "Mocodes":"",
    "Crm Cd 1":None,
    "Crm Cd 2":None,
    "Crm Cd 3":None,
    "Crm Cd 4":None,

}


crimes_schema_name = "CrimesSchema"
pd.set_option('display.max_colwidth', None)
pd.set_option('display.max_rows', None)

#wait I can most of this shit here!
all_data = pd.read_csv("project01Data.csv",dtype=str)
all_data = createDefaults(all_data,default_values_dict,all_data.columns)
all_data["Vict Age"] = all_data["Vict Age"].apply(lambda x: "0" if (pd.isnull(x) or int(x)<0 or int(x)>130) else x)
all_data["Vict Sex"] = all_data["Vict Sex"].apply(lambda x: "X" if (x!="X" and x!="F" and x!="M") else x)

print(len(all_data))
# I am thinking of creating the table data outside the insertion and then mass-inserting them
# This is probably a bit faster without constantly fetching etc
loc_data = all_data[["LOCATION","Cross Street","LON","LAT"]].drop_duplicates()
loc_data = addUuid(loc_data,"loc_id")
premis_data = keepUniqueCdDescr(all_data,"Premis Cd","Premis Desc")
weapon_data = keepUniqueCdDescr(all_data,"Weapon Used Cd","Weapon Desc")
crimeCodes_data = keepUniqueCdDescr(all_data,"Crm Cd","Crm Cd Desc")
inv_status_data = keepUniqueCdDescr(all_data,"Status","Status Desc")
areas_data = keepUniqueCdDescr(all_data,"AREA","AREA NAME")
rep_distr_data = all_data["Rpt Dist No"].drop_duplicates().dropna().tolist()
rep_distr_data = [(x,) for x in rep_distr_data]

crimes_junction = all_data[["DR_NO","Crm Cd","Crm Cd 1","Crm Cd 2","Crm Cd 3","Crm Cd 4"]].drop_duplicates()
crimes_junction_list = []
for index,row in crimes_junction.iterrows():
    for column in crimes_junction.columns:
        if column != "DR_NO":
            if column == "Crm Cd":
                crimes_junction_list.append((row["DR_NO"],row[column],True))
            else:
                if row[column] != None:
                    if row[column] != row["Crm Cd"]:
                        if row[column] not in [t[0] for t in crimeCodes_data]:
                            crimeCodes_data.append((row[column],"Unknown Crime"))
                        crimes_junction_list.append((row["DR_NO"],row[column],False))
# simple tables


#maybe I can populate null values here...

victim_data = all_data[["Vict Age","Vict Descent","Vict Sex"]]
# lets do an addition here to change all nulls to default values and then drop duplicates
victim_data = victim_data.drop_duplicates()
victim_data = addUuid(victim_data,"victim_id")


print(len(rep_distr_data))
# composite tables
crimes_data = all_data[["DR_NO","Date Rptd","DATE OCC","TIME OCC","AREA","Rpt Dist No","Mocodes","Weapon Used Cd","Status","Premis Cd"]].drop_duplicates()
#find the corresponding loc_id
crimes_data = findIds(crimes_data,"loc_id",all_data,loc_data,["LOCATION","Cross Street","LON","LAT"])
victim_junction = findVictms(crimes_data,victim_data,all_data,["Vict Age","Vict Descent","Vict Sex"])
victim_junction = victim_junction.values.tolist()
crimes_data = crimes_data.values.tolist()
loc_data = loc_data.values.tolist()
victim_data = victim_data.values.tolist()

print(len(crimes_data))
# junction tables



#add to the tables
print(len(premis_data))
db_con = psy.connect(host="localhost",dbname="ProjectCrimesDB",user="kriktze",password="Cem8flc1221",port=5432)
db_con.set_session(autocommit=False)
cur = db_con.cursor()
#inserting simple tables
cur.execute(f'DELETE FROM "CrimesSchema".crimes')
cur.execute(f'DELETE FROM "CrimesSchema".location')
cur.execute(f'DELETE FROM "CrimesSchema".crime_premis')
cur.execute(f'DELETE FROM "CrimesSchema".crime_weapons')
cur.execute(f'DELETE FROM "CrimesSchema".crime_codes')
cur.execute(f'DELETE FROM "CrimesSchema".crimes_inv_status')
cur.execute(f'DELETE FROM "CrimesSchema".areas')
cur.execute(f'DELETE FROM "CrimesSchema".report_district')
cur.execute(f'DELETE FROM "CrimesSchema".victims')

db_con.commit()


executeTheQuery(cur,nameTable("crime_premis"),premis_data,['premis_code','premis_desc'],1000)
executeTheQuery(cur,nameTable("crime_weapons"),weapon_data,['wpn_code','wpn_desc'],1000)
executeTheQuery(cur,nameTable("crime_codes"),crimeCodes_data,['crm_code','crm_descr'],1000)
executeTheQuery(cur,nameTable("crimes_inv_status"),inv_status_data,['status_code','status_desc'],1000)

executeTheQuery(cur,nameTable("areas"),areas_data,['area_code','area_name'],1000)
executeTheQuery(cur,nameTable("report_district"),rep_distr_data,['district_code'],1000)

executeTheQuery(cur,nameTable("victims"),victim_data,['victim_age','victim_descent','victim_sex','victim_id'],1000)
insertLocData(cur,loc_data,1000)

executeTheQuery(cur,nameTable("crimes"),crimes_data,["drno","date_rptd","date_occ","time_occ","area_code","reporting_distrc","mocodes","wpn_code","inv_status","premis_code","loc_id"],10000)
executeTheQuery(cur,nameTable("crimes_commited"),crimes_junction_list,["drno","crime_code","ismaincrime"],10000)
executeTheQuery(cur,nameTable("crimes_victims"),victim_junction,["drno","victim_id"],10000)

#inserting composite tables

db_con.commit()
cur.close()
db_con.close()

