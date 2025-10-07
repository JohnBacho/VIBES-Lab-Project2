import pandas as pd

'''
df1 = pd.read_csv("Python Scripts/2025_4_3_1124_0_camera_tracker.csv")
#Change types to match other csv
df1['SubjectID']=df1['SubjectID'].astype("int64")
df1['LocalTime']=df1['LocalTime'].astype("object")
df1['UnityTime']=df1['UnityTime'].astype("float64")
df1['Step']=df1['Step'].astype("int64")
'''

df2 = pd.read_csv("/Users/albertselby/Documents/GitHub/VR-LAB/data/NewData/2025_7_2_1010_0_eyetracker.csv")

# Grabs SubjectID and Date to be used in filename creation.
subject_id = df2["SubjectID"].iloc[0]
date = df2["Date"].iloc[0]

look_times = []
current_object = None
start_time = None

# Calculates how long a game object was looked at
for idx in range(len(df2)):
    row = df2.iloc[idx]
    game_object = row['GameObjectInFocus']
    unity_time = row['UnityTime']

    if current_object is None:
        current_object = game_object
        start_time = unity_time
        continue

    if game_object != current_object:
        # Use the previous row's UnityTime as the actual end time
        end_time = df2.iloc[idx - 1]['UnityTime']
        duration = end_time - start_time
        look_times.append({
            'GameObject': current_object,
            'StartTime': start_time,
            'EndTime': end_time,
            'Duration': duration
        })

        current_object = game_object
        start_time = unity_time

# Handle last object
if current_object is not None and start_time is not None:
    end_time = df2.iloc[-1]['UnityTime']
    duration = end_time - start_time
    look_times.append({
        'GameObject': current_object,
        'StartTime': start_time,
        'EndTime': end_time,
        'Duration': duration
    })

# Create a new DataFrame with look times
look_times_df = pd.DataFrame(look_times)
# Initialize new columns
df2['LookedGameObject'] = None
df2['LookDuration'] = None

for idx, row in df2.iterrows():
    unity_time = row['UnityTime']
    focus_object = row['GameObjectInFocus']

    match = look_times_df[
        (look_times_df['GameObject'] == focus_object) &
        (look_times_df['StartTime'] <= unity_time) &
        (unity_time <= look_times_df['EndTime'])
    ]

    if not match.empty:
        df2.at[idx, 'LookedGameObject'] = match.iloc[0]['GameObject']
        df2.at[idx, 'LookDuration'] = match.iloc[0]['Duration']
        
# writes file name and saves file
filename_full = f"Subject{int(subject_id)}Date{str(date)}.csv"
df2.drop(columns=['LookedGameObject'], inplace=True)
df2.to_csv(filename_full, index=False)

filename_reduced = f"Reduced-Subject{int(subject_id)}Date{str(date)}.csv"
step = df2[(df2['Stage'] != "InstructionPhase") & (df2['Stage'] == "InterTrial")]
step.to_csv(filename_reduced, index=False)

# Computes average eye movement distance per (Phase, TrialNumber)
import numpy as np

def compute_avg_eye_movement(input_filename):
    df = pd.read_csv(input_filename)
    results = []

    if "Phase" in df.columns and "TrialNumber" in df.columns:
        grouped = df.groupby(["Phase", "TrialNumber"])
        for (phase, trial), group in grouped:
            valid_group = group[["GazeHitPointX", "GazeHitPointY", "GazeHitPointZ"]].dropna()
            x = valid_group["GazeHitPointX"].astype(float).values
            y = valid_group["GazeHitPointY"].astype(float).values
            z = valid_group["GazeHitPointZ"].astype(float).values

            dx = np.diff(x)
            dy = np.diff(y)
            dz = np.diff(z)

            step_distances = np.sqrt(dx**2 + dy**2 + dz**2)
            avg_step_distance = np.mean(step_distances) if len(step_distances) > 0 else np.nan

            group.loc[:, "AvgEyeMoveDist"] = avg_step_distance
            results.append(group)

        result_df = pd.concat(results)
        result_df.to_csv(input_filename, index=False)
    else:
        print(f"Missing 'Phase' and/or 'TrialNumber' columns in {input_filename}.")

compute_avg_eye_movement(filename_full)
compute_avg_eye_movement(filename_reduced)