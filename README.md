# Dynamic Music System for Unity

A flexible and easy-to-use dynamic music system for Unity games that allows for seamless music transitions based on player location and game events.

## Features

- Trigger-based music system with smooth volume transitions
- Support for both tag and layer-based trigger detection
- Background music with customizable transition speeds
- Multiple audio source management
- Integration with Unity's Audio Mixer system
- Event system for music state changes
- Random clip selection from arrays

## Requirements

- Unity (Compatible with recent versions)
- NaughtyAttributes package for enhanced Unity inspector functionality

## Installation

1. Import the DynamicMusicSystem folder into your Unity project
2. Ensure you have NaughtyAttributes package installed in your project

## Setup

1. Create an empty GameObject in your scene
2. Add a Collider component (required) and set it as trigger
3. Add the DynamicMusicSystem script to the GameObject
4. Configure the settings in the inspector

## Settings

### First Clip
- `PlayFirstClip`: Enable to play initial background music
- `FirstClip`: Array of audio clips for initial background music

### General
- `AudioSourceBackgroundMusic`: Main AudioSource for background music
- `AudioMixer`: AudioMixerGroup for volume control

### Settings
- `SpeedChangeMusic`: Speed of music volume transition (0.1-10)
- `CheckTag`: Enable tag-based trigger detection
- `tagCheck`: Tag to check (default: "Player")
- `CheckLayer`: Enable layer-based trigger detection
- `layerCheck`: Layer to check (default: "MusicLayer")
- `PlayingFirstClipAfterExit`: Return to first clip after trigger exit

### Events
- `OnPlayNewClip`: Event triggered when new music starts playing

## Usage

1. Set up your trigger zones with colliders
2. Configure the trigger detection method (tag or layer)
3. Add your audio clips to the FirstClip array if needed
4. Set up your AudioMixer if you want to use mixer groups
5. Adjust the transition speed to your preference

## Public Methods

- `SetNewClip(AudioClip audioClip)`: Manually set a new clip to play

## Notes

- The system automatically handles audio source creation and cleanup
- Volume transitions are handled smoothly in the background
- Multiple music zones can be created in the same scene
