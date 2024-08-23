import os
import json
import re

war = [
        {"BillboardHuge01": "ParadoxPoster"},
        {"BillboardLarge01": "ParadoxPoster"},
        {"BillboardMedium01": "ParadoxPoster"},
        {"BillboardRoundLarge01": "ParadoxLogo"},
        {"BillboardRoundMedium01": "ParadoxLogo"},
        {"BillboardRoundSmall01": "ParadoxLogo"},
        {"BillboardSmall01": "CS2Poster"},
        {"BillboardWallHuge01": "PA2Poster"},
        {"BillboardWallLarge01": "CS2Poster"},
        {"BillboardWallLarge03": "CS2Poster"},
        {"BillboardWallMedium01": "CS2Poster"},
        {"BillboardWallSmall01": "FoundryPoster"},
        {"BillboardWallSmall03": "ParadoxPoster"},
        {"AStand02_Top": "ParadoxOnSale"},
        {"GasStationPylon01_Up": "LBYPoster"},
        {"GasStationPylon02_Up": "MilleniaPoster"},
        {"GasStationPylon01_Down": "ParadoxGamesThumb2024"},
        {"GasStationPylon02_Down": "ParadoxGamesThumb2024"},
        {"GasStationPylon03": "FoundryPoster"},
        {"PosterHuge01": "ParadoxGamesCollage2024"},
        {"PosterHuge02": "CS2Poster"},
        {"PosterLarge01": "ParadoxPoster2022"},
        {"PosterLarge02": "FoundryPoster"},
        {"PosterMedium01": "ParadoxGamesThumb2024"},
        {"PosterMedium02": "LBYPoster"},
        {"PosterSmall01": "ParadoxCareerCall"},
        {"PosterSmall02": "CS1Poster"},
        {"Screen01": "ParadoxSteamSale2024-80percent"},
        {"Screen02": "ParadoxSteamSale2024-80percent"},
        {"SignFrontwayLarge01": "ParadoxLogoWideBlackBG"},
        {"SignFrontwayMedium01": "ParadoxLogoWideBlackBG"},
        {"SignFrontwaySmall01": "ParadoxLogoWideBlackBG"},
        {"SignNeonLarge01": "ParadoxLogoWideBlackBG"},
        {"SignNeonLarge02": "CS2Poster"},
        {"SignNeonMedium01": "ParadoxLogoWideBlackBG"},
        {"SignNeonMedium02": "CS2Poster"},
        {"SignNeonSmall01": "ParadoxLogoWideBlackBG"},
        {"SignNeonSmall02": "CS2Poster"},
        {"SignSidewayLarge01": "ParadoxLogoWideBlackBG"},
        {"SignSidewayMedium01": "ParadoxLogoWideBlackBG"},
        {"SignSidewaySmall01": "ParadoxLogoSquareBlackBG"},
        {"Stand01": "ParadoxSteamSale2024-80percent"},
    ]

def clean_json(json_string):
    # Remove or handle unique values like $fstrref:"UnityGUID:..."
    cleaned_json = re.sub(r'\$fstrref:"[^"]*"', 'null', json_string)
    cleaned_json = re.sub(r'([\d]+|"[\d]+\|UnityEngine.Color, UnityEngine.CoreModule"),\s*\d+.\d+,\s*\d+.\d+,\s*\d+.\d+,\s*\d+', '"null"', cleaned_json)
    
    return cleaned_json

# def preprocess_json(json_string,filename):
#     # Replace the specific pattern with 'var something'
#     pattern = re.compile(r'(\s*\]\s*\},\s*"m_Meshes":\s*\{\s*"\$id":)')
#     filenameRaw = filename.replace('.Prefab','')
#     theme = filenameRaw.split("_")[0]
#     ui = f""",
#             {{
#                 "$id": 1000,
#                 "$type": "1000|Game.Prefabs.EditorAssetCategoryOverride, Game",
#                 "name": "EditorAssetCategoryOverride",
#                 "active": true,
#                 "m_IncludeCategories": {{
#                     "$id": 1001,
#                     "$type": "1001|System.String[], mscorlib",
#                     "$rlength": 1,
#                     "$rcontent": [
#                         "StarQ/Buildings/Office/Medium Office/{theme}"
#                     ]
#                 }},
#                 "m_ExcludeCategories": {{
#                     "$id": 1002,
#                     "$type": 1001,
#                     "$rlength": 0,
#                     "$rcontent": [
#                     ]
#                 }}
#             }}"""
#     preprocessed_json = pattern.sub(ui + r'\1', json_string)
#     return preprocessed_json

def count_rcontent_in_json(json_string):
    rcontent_count = 0 

    try:
        # Clean the JSON string for parsing
        cleaned_json_string = clean_json(json_string)
        
        # Parse the cleaned JSON content
        data = json.loads(cleaned_json_string)

        
        # Check if 'components' exists in the JSON data
        if 'components' in data:
            components = data['components']
            
            # Check if '$rcontent' exists in 'components'
            if '$rcontent' in components:
                rcontent = components['$rcontent']
                
                # Count the number of items in the rcontent array
                rcontent_count = len(rcontent)

    except json.JSONDecodeError:
        # Handle JSON decode error (if the file is not a valid JSON)
        print("Invalid JSON format.")
    
    return rcontent_count

# Get the current working directory
cwd = os.getcwd()

# Walk through all directories and subdirectories in the current working directory
for root, dirs, files in os.walk(cwd):
    for filename in files:
        # Check if the file is a .Prefab file
        if filename.endswith('.Prefab'):
            # Construct the full file path
            filepath = os.path.join(root, filename)
            
            # Open and read the content of the file
            with open(filepath, 'r') as file:
                file_lines = file.readlines()
                
            # Read the file content as a string
            with open(filepath, 'r') as file:
                json_string = file.read()

            # preprocessed_json_string = preprocess_json(json_string,filename)
            preprocessed_json_string = json_string
            # Get the rcontent count
            rcontent_count = count_rcontent_in_json(preprocessed_json_string)
            
            if rcontent_count != 0:

                # Replace the 9th line with the rlength info
                if len(file_lines) >= 9:
                    file_lines[8] = f'        "$rlength": {rcontent_count},\n'
                
                # Write the modified lines back to the file
                # with open(filepath, 'w') as file:
                #     file.writelines(file_lines)
                print(f"File: {filename}, rcontent count: {rcontent_count}")

print("Files updated successfully.")
