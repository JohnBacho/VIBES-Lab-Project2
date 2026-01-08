# Virtual Immersive Behavioral Sciences (VIBES) Lab – **Project 2: VR Sports Gambling Study**

<div align="center">
<img width="450" alt="VIBES Lab Logo" src="https://github.com/user-attachments/assets/89824d3a-373a-448f-9b5c-256f4c459466" />

[![License: CC BY-NC 4.0](https://img.shields.io/badge/License-CC%20BY--NC%204.0-lightgrey.svg)](https://creativecommons.org/licenses/by-nc/4.0/)
[![Unity Version](https://img.shields.io/badge/Unity-2023.1.5f1-blue.svg)](https://unity.com/)
[![Platform](https://img.shields.io/badge/Platform-VR-brightgreen.svg)](https://github.com/)

</div>

---

## 📖 Overview

**Project 2** is a VIBES Lab research initiative investigating **decision-making and behavioral patterns in simulated sports gambling and slot machine environments within VR**. This study explores how different gambling modalities influence risk evaluation, reward sensitivity, and choice behavior through immersive virtual experiences.

Building on the VR infrastructure established in Project 1, this project introduces interactive gambling environments that enable precise measurement of:

- **Risk evaluation patterns** across gambling types
- **Reward sensitivity** and payout response
- **Choice dynamics** in betting scenarios
- **Visual attention** through eye-tracking during decision-making
- **Pupillary response** as a physiological indicator of arousal

---

## 🎮 Key Features

### **Dual Gambling Modality System**
- **Sports Parlay Betting**: Multi-leg parlay construction with realistic odds and detailed stat cards
- **Slot Machine Gambling**: Symbol-based slot mechanics with configurable multipliers
- **Counterbalanced Design**: Switchable order (Slots→Parlay or Parlay→Slots) for experimental control

### **Comprehensive Data Collection**
The system captures trial-level data including:
- Outcome (Win/Loss)
- Gambling type (Sports/Slots)
- Bet amount and payout
- Real-time wallet tracking (separate for each modality)
- **Trial-averaged pupil size** (mean of all samples during trial)
- **Combined eye pupil size** (average of left and right pupils)
- **Parlay specifics**: Total odds, number of legs, individual team selections and odds (up to 5 legs)

### **Enhanced User Experience**
- **Ecologically valid UI** designed to mirror real gambling interfaces
- **Audio feedback system** with UI sounds and time-pressure music
- **Interactive stat cards** for informed parlay decisions
- **Comprehensive tutorials** for participant onboarding
- **Error prevention** to ensure data integrity

### **Demo Available**
[Watch the system in action](https://youtu.be/4wiufuqs_OQ?si=7awEUS9G1JfmKQzM)

---

## 🛠️ Technology Stack

| Component | Technology |
|-----------|-----------|
| Game Engine | Unity 2023.1.5f1 |
| VR Hardware | HTC VIVE Pro Eye |
| Eye Tracking | SRanipal SDK |
| Data Framework | SimpleOmnia |
| VR Platform | SteamVR |
| Programming | C# |
| Data Processing | Web-based CSV Processor |

---

## 📊 Research Design

### **Experimental Structure**
- **Within-subjects design**: Each participant experiences both gambling types
- **Counterbalanced order**: Configurable Slots→Sports or Sports→Slots
- **Trial-based**: 15 gambling trials per session with 2 in the middle
- **Financial tracking**: Separate wallet systems maintain realism for each modality

### **Measured Variables**
**Behavioral:**
- Bet amounts and frequency
- Parlay construction choices (leg count, odds selection, stat card usage)
- Win/loss outcomes
- Decision latency

**Physiological:**
- Combined pupil diameter (left + right average)
- Trial-averaged pupil size (arousal proxy)
- Gaze patterns during selection
- Head movement dynamics

**Contextual:**
- Gambling type
- Trial sequence
- Accumulated wins/losses
- Total parlay risk (odds calculation)

---

## 📥 Getting Started

### **Prerequisites**
- Unity 2023.1.5f1
- HTC VIVE Pro Eye with eye-tracking calibration
- SteamVR runtime
- SRanipal SDK (v1.3.6.8 or higher)

### **Installation**
```bash
git clone https://github.com/JohnBacho/VIBES-Lab-Project2.git
cd VIBES-Lab-Project2
```

1. Open the project in **Unity 2023.1.5f1**
2. Install SRanipal runtime and SDK (see Project 1 documentation)

### **Running an Experiment**
1. Launch Program
2. Auto starts eye calibration
3. Data automatically saves to `Assets/Experiments/ProgramName/`

---

## 📈 Data Output & Processing

### **CSV Structure**
Each session generates a CSV with the following columns:

| Column | Description |
|--------|-------------|
| `ProgramName` | Unique participant identifier |
| `Outcome` | Win/Loss result |
| `GamblingType` | Sports/Slots |
| `Bet` | Amount wagered |
| `Payout` | Winnings (if applicable) |
| `Wallet` | Current balance |
| `TrialAveragePupilSize` | Mean pupil diameter across entire trial |
| `Total_Odds` | Combined parlay odds (risk metric) |
| `Total_Legs` | Number of parlay selections |
| `Parlay1-5_Team` | Team names for each leg |
| `Parlay1-5_Odds` | Individual odds per leg |

### **Data Processing Tool**
Raw CSV files can be processed using our companion web application:

- **Repository**: [VIBES-Lab-Project2-CSV-Processor](https://github.com/JohnBacho/Vibes-Lab-Project2-CSV-Processor)
- **Web Interface**: [https://johnbacho.github.io/VIBES-Lab-Project2-CSV-Processor/](https://johnbacho.github.io/VIBES-Lab-Project2-CSV-Processor/)

The processor automatically:
- Generates unique filenames using ProgramName, date, and time
- Removes the 16th trial (program termination marker)
- Cleans unnecessary columns
- Formats data for easy statistical analysis 

---

## 👥 Core Team

| Name | Department |
|------|-----------|
| **Dr. Brian Thomas** | Psychology |
| **John Bacho** | Computer Science |
| **Lauren Dunlap** | Psychology |
| **Albert Selby** | Computer Science / Data Science |
| **Marissa Brigger** | Neuroscience |
| **Alexa Gossett** | Neuroscience / Psychology |
| **Jace Lander** | Software Engineering |
| **Corey Schwarz** | Computer Science / Data Science |
| **Olivia Mullins** | Neuroscience |

---

## 🔬 Research Applications

This platform enables investigation of:
- **Comparative gambling psychology**: Sports betting vs. chance-based gambling
- **Decision-making under uncertainty**: How odds complexity affects choices
- **Arousal and risk-taking**: Pupillometry during gambling decisions
- **Loss-chasing behavior**: Wallet dynamics and bet escalation
- **Information seeking**: Stat card usage patterns in sports betting

---

## 📝 Changelog (as of January 5, 2026)

### **Major Features**
- Added switchable gambling context order (Slot→Parlay or Parlay→Slot)
- Implemented separate wallet systems for each gambling type
- Overhauled Sports Parlay UI for ecological validity
- Added interactive stat cards for each parlay leg with tutorial
- Converted slot reels from numbers to symbols
- Implemented per-trial slot payout multipliers

### **Data Collection Enhancements**
- Added selected parlay details (Team Names & Odds) to CSV output
- Implemented `Total_Odds` column for risk assessment
- Implemented `Total_Legs` column for parlay complexity tracking
- Added `combinedEyePupilSize` (left + right pupil average)
- Added `TrialAveragePupilSize` (temporal average across trial)
- Renamed `currentbet` → `bet` and updated processors accordingly
- Changed `subjectID` → `ProgramName` for clarity

### **CSV Processor Improvements**
- Redesigned web interface with enhanced UI 🌻
- Implemented unique filename generation (ProgramName + timestamp)
- Automatic removal of 16th trial (termination marker)
- Column cleanup for streamlined analysis
- Support for new data fields (pupil metrics, parlay details)

### **User Experience**
- Revamped UI buttons for better interaction
- Added comprehensive audio feedback system
- Implemented time-pressure music after decision threshold
- Added safeguards preventing unintended interactions during startup
- Enhanced tutorial system for stat card usage

### **Backend Improvements**
- Rewrote parlay loss logic for increased realism (weighted negative outcomes)
- Added program ending logic
- Implemented extensive error handling
- Multiple bug fixes for stability

---

## 🙏 Acknowledgments

- **Justin Kasowski** – SimpleOmnia framework development
- **HTC Corporation** – VIVE Pro Eye and SRanipal SDK
- **Unity Asset Store** – Environmental and UI assets
- **Baldwin Wallace University** – Institutional support

---

## 📄 License

This project is licensed under **Creative Commons Attribution-NonCommercial 4.0 International (CC BY-NC 4.0)**.

Includes SimpleOmnia framework components under the same license.

---

## 📧 Contact

- **Lead Developer:** [jbacho22@bw.edu](mailto:jbacho22@bw.edu)
- **Lab Inquiries:** [Dr. Brian Thomas](mailto:bthomas@bw.edu)
- **Issues & Contributions:** Use the GitHub Issues tab

---

## 🔗 Related Repositories

- **CSV Data Processor**: [VIBES-Lab-Project2-CSV-Processor](https://github.com/JohnBacho/Vibes-Lab-Project2-CSV-Processor)
- **Project 1 (Foundation)**: [Link to Project 1 repository](https://github.com/JohnBacho/VIBES-Lab-Project1)

---

<div align="center">

**Made with ❤️ by the VIBES Lab Team**

*Advancing behavioral science through immersive technology*

</div>
