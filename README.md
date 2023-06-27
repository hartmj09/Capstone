# Desktop piano application
This is a piano desktop application that maps keyboard keys to musical notes and plays the sound of the notes played. The user can choose between an Accoustic or Synth piano. It also allows the user to record the played notes and creates a MusicXML file from what was played. 
All the user needs to do is change the file path of the const string xmlfile variable (located in the MusicXMLWriter class) to their desired file path. For the extension of the file path use .xml. This is where the MusicXML will be saved on the computer. After the user
can import the file to 3rd party software programs (such as Sibelius or MuseScore 4) and it will display what was played into musical sheet notation. 


## keboard mappings
Below is an outline that shows which keyboard keys are mapped to which notes.

keyboard key => musical note + the octave

A => C 4 (middle C)

W => C-Sharp 4 (D-Flat 4)

S => D 4

E => D-Sharp 4 (E-flat 4)

D => E4

F => F4

T => F-sharp 4 (G-flat 4)

G => G4

Y => G-sharp 4 (A-flat 4)

H => A4

U => A-Sharp 4 (B-flat 4)

J => B4

K => C5

O => C-Sharp 5 (D-flat 5)

L => D5

P => D-Sharp 5 (E-Flat 5)

## Current Iteration of the Project
When recording this applicatoin currently recognizes single notes and chords. It can also recognize tied notes for single notes and script them correctly following MusicXML notation. 

Currently the application does not recognize rests or tied notes for chords.






