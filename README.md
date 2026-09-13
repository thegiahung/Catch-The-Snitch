Link to the Video: https://youtube.com/shorts/mvlCqOB5CXc
# Catch the Snitch — VR Exergame with Adaptive Movement

Undergraduate thesis project, Bachelor of Advanced Computing, University of Sydney (2024).

## Overview
An exergame built for Oculus VR in Unity. Players physically reach out to
catch a moving object ("the snitch") to encourage sustained physical
activity through gameplay rather than a traditional structured workout.

## My contribution
- Designed and implemented the snitch's movement using a Lévy flight
  algorithm (adapted from an open-source C# implementation), generating
  step lengths from a Lévy distribution so movement alternates between
  short, frequent steps and occasional long jumps rather than a
  predictable path.
- Built the boundary and collision logic constraining the snitch's
  movement within the play area and away from the player.
- Integrated a hand motion prediction model (built by a teammate) into
  the overall game architecture and gameplay loop, so predicted hand
  position updates every frame to reduce perceived input latency.
- Built the core Unity game loop: GameManager, scoring, session timer,
  and difficulty conditions (Normal, Hard, Predictive).

## Tech stack
Unity, C#, Oculus SDK (OVRCameraRig, Oculus Touch controllers).

## How it works
The snitch's target position updates continuously using a Lévy
distribution (β = 0.5), giving it a non-repetitive, semi-random movement
path. This is combined with a hand prediction model that estimates the
player's hand position ahead of the tracked input, reducing the gap
between the player's physical movement and what they see in the
headset. Difficulty is varied by adjusting the beta parameter controlling
step-size distribution.

## Evaluation
Small-scale pilot sessions (1-minute and 10-minute formats) were run to
observe player engagement and gameplay feel across different beta
settings, using in-game metrics (score, win rate) and player feedback.
The report notes the pilot's small sample size as a limitation, and
recommends a larger, more demographically varied study as future work.

## Limitations / future work
- Small pilot sample size, noted explicitly in the report.
- No formal statistical testing of perceived exertion (Borg RPE) or
  actual exertion was completed against a control condition.
- Haptic feedback was scoped out of this version.
