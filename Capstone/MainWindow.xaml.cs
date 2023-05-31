using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Media;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using NAudio.Midi;




namespace Capstone
{
    public class Note : IComparable<Note>
    {
        public Key pitch;
        public DateTime startOfKeyPress; 
        public DateTime endOfKeyPress;  
        private TimeSpan duration;
        private NoteType typeOfNote;
        private string nameOfNote;
        private Alteration alter;
        private string octave;
        public int weight;
        private int tied; //used for tied notes
        
        //dictionary for if a note is a flat, natural, or sharp
        private static Dictionary<Key, Note.Alteration> alterationNotes = new Dictionary<Key, Note.Alteration>();

        //dictionary for name of note (C,D,E..etc
        private static Dictionary<Key, string> musicalNoteNames = new Dictionary<Key, string>();

        //dictionary for name of the note Octave
        private static Dictionary<Key, string> musicalNoteOctave = new Dictionary<Key, string>();

        public static void initializeMusicalOctaveDictionary()
        {
            musicalNoteOctave.Add(Key.A, "4"); //C4
            musicalNoteOctave.Add(Key.W, "4"); //C4#
            musicalNoteOctave.Add(Key.S, "4"); //D4
            musicalNoteOctave.Add(Key.E, "4"); //Eb4 or D4#
            musicalNoteOctave.Add(Key.D, "4"); //E4
            musicalNoteOctave.Add(Key.F, "4"); //F4
            musicalNoteOctave.Add(Key.T, "4"); //Gb4 or F4#
            musicalNoteOctave.Add(Key.G, "4"); //G4
            musicalNoteOctave.Add(Key.Y, "4"); //Ab4 or G4#
            musicalNoteOctave.Add(Key.H, "4"); //A4
            musicalNoteOctave.Add(Key.U, "4"); //Bb4 or A4#
            musicalNoteOctave.Add(Key.J, "4"); //B4
            musicalNoteOctave.Add(Key.K, "5"); //C5
            musicalNoteOctave.Add(Key.O, "5"); //Db5 or C5#
            musicalNoteOctave.Add(Key.L, "5"); //D5
            musicalNoteOctave.Add(Key.P, "5"); //Eb5 orD5#

        }


        public static void initializeMusicalNoteNamesDictionary()
        {
            musicalNoteNames.Add(Key.A, "C"); //C4
            musicalNoteNames.Add(Key.W, "C"); //C4#
            musicalNoteNames.Add(Key.S, "D"); //D4
            musicalNoteNames.Add(Key.E, "D"); //Eb4 or D4#
            musicalNoteNames.Add(Key.D, "E"); //E4
            musicalNoteNames.Add(Key.F, "F"); //F4
            musicalNoteNames.Add(Key.T, "F"); //Gb4 or F4#
            musicalNoteNames.Add(Key.G, "G"); //G4
            musicalNoteNames.Add(Key.Y, "G"); //Ab4 or G4#
            musicalNoteNames.Add(Key.H, "A"); //A4
            musicalNoteNames.Add(Key.U, "A"); //Bb4 or A4#
            musicalNoteNames.Add(Key.J, "B"); //B4
            musicalNoteNames.Add(Key.K, "C"); //C5
            musicalNoteNames.Add(Key.O, "C"); //Db5 or C5#
            musicalNoteNames.Add(Key.L, "D"); //D5
            musicalNoteNames.Add(Key.P, "D"); //Eb5 orD5#

        }


        public static void initializeAlterationNotesDictionary()
        {
            alterationNotes.Add(Key.A, Note.Alteration.natural); //C4
            alterationNotes.Add(Key.W, Note.Alteration.sharp); //C4#
            alterationNotes.Add(Key.S, Note.Alteration.natural); //D4
            alterationNotes.Add(Key.E, Note.Alteration.sharp); //Eb4 or D4#
            alterationNotes.Add(Key.D, Note.Alteration.natural); //E4
            alterationNotes.Add(Key.F, Note.Alteration.natural); //F4
            alterationNotes.Add(Key.T, Note.Alteration.sharp); //Gb4 or F4#
            alterationNotes.Add(Key.G, Note.Alteration.natural); //G4
            alterationNotes.Add(Key.Y, Note.Alteration.sharp); //Ab4 or G4#
            alterationNotes.Add(Key.H, Note.Alteration.natural); //A4
            alterationNotes.Add(Key.U, Note.Alteration.sharp); //Bb4 or A4#
            alterationNotes.Add(Key.J, Note.Alteration.natural); //B4
            alterationNotes.Add(Key.K, Note.Alteration.natural); //C5
            alterationNotes.Add(Key.O, Note.Alteration.sharp); //Db5 or C5#
            alterationNotes.Add(Key.L, Note.Alteration.natural); //D5
            alterationNotes.Add(Key.P, Note.Alteration.sharp); //Eb5 or D5#

        }


        public enum Alteration
        {
            flat = -1, natural, sharp
        }
        public enum NoteType
        {
            whole, half, quarter, eighth, sixteenth, thirtySecond, sixtyFourth

        }

        public int weightOfNote()
        {
            int noteWeight = 0;
            int baseOfPower = 2;
            int exponent = 6 - (int)typeOfNote;

            //power returns double,we round to nearest integral value and cast as int
            noteWeight = (int)Math.Round(Math.Pow(baseOfPower, exponent));

            return noteWeight; 
        }
        //calculates the type of note based on the notes weight (i.e. 32 = half note)
        public void setTypeOfNoteBasedOnWeight()
        {
            int noteWeight = weight;
            int baseOfPower = 2;

            //log base 2 x = weight
            int exponentOfTypeOfNote = (int)Math.Round(Math.Log(noteWeight, baseOfPower));
            int calculationTypeOfNote = 6 - exponentOfTypeOfNote;
            typeOfNote = (NoteType)calculationTypeOfNote;
        }
        



        public Note(Key inputPitch, DateTime inputStartOfKeyPress, DateTime inputEndOfKeyPress)
        {
            pitch = inputPitch;
            startOfKeyPress = inputStartOfKeyPress;
            endOfKeyPress = inputEndOfKeyPress;
            duration = endOfKeyPress - startOfKeyPress;        
            findTypeOfNote(); //implicitly passes note
            alter = alterationNotes[pitch];
            nameOfNote = musicalNoteNames[pitch];
            octave = musicalNoteOctave[pitch];
            weight = weightOfNote();
            tied = 0;

         
        }
        private void findTypeOfNote()
        {
            //assuming 4 4 time and 120 bpm
            double durationOfWholeNoteInSeconds = 2;
            double powerOfDuration = Math.Log(duration.TotalSeconds / durationOfWholeNoteInSeconds, durationOfWholeNoteInSeconds);
            int interPowerOfDuration = -(int)Math.Round(powerOfDuration); //rounds to nearest whole integer and negate it for enum

            //think about long note presses-maybe using a sustain pedal or parse the note
            typeOfNote = (NoteType)interPowerOfDuration;

        }

        public int getTied() { return tied; }

        public void setTied(int tied) {  this.tied = tied; }



        public string getAlterationOfNote()
        {
            int intAlter = (int)alter;
            if (intAlter == 0) 
            {
                return null;
            }
            string stringAlter = intAlter.ToString();
            return stringAlter;
        }
        public string getNameOfNote() 
        {            
            return nameOfNote;  
        }
        public string getOctave()
        { 
            return octave;
        }

        public string getDurationAsString()
        {
            return duration.ToString();
        }
        public string getTypeOfNoteAsString()
        {
            if (typeOfNote == NoteType.sixteenth)
                return "16th";
            else if (typeOfNote == NoteType.thirtySecond)
                return "32nd";
            else if (typeOfNote == NoteType.sixtyFourth)
                return "64th";
            else
                return typeOfNote.ToString();
        }
        public int CompareTo(Note otherNote)
        {
            //check to see if this is correct. stop same key exception from hitting
            if (-weight.CompareTo(otherNote.weight) == 0)
                return 1;
            else
            {
                return -weight.CompareTo(otherNote.weight);
            }

        }

    }

    public partial class MainWindow : Window
    {
        //maps keyboard key to buttons
        private Dictionary<Key, Button> PianoKeys = new Dictionary<Key, Button>();

        //stores a sortedlist of notes and key = when note was pressed (played)
        private Dictionary<DateTime, SortedList<Note, Note>> playedNotes = new Dictionary<DateTime, SortedList<Note,Note>>();

        //gets the start of when a key was pressed (played)
        private Dictionary<Key, DateTime?> startTimeOfNote = new Dictionary<Key, DateTime?>();

        //dictionary to stop key_down event from constantly firing
        private Dictionary<Key, bool> keyEnabledDictionary = new Dictionary<Key, bool>();

        //dictionary for piano sounds
        private Dictionary<Key, int> pianoSounds = new Dictionary<Key, int>();

        //creates midi instance to create sound-0 argument is for built in midi on computer
        private MidiOut midiOut = new MidiOut(0);

        //selects which instrument the midi will play
        private int channel = 1;

        //toggle variable for record button
        private bool recordOn = true;

        


        public MainWindow()
        {
            InitializeComponent();
            ResizeMode = ResizeMode.NoResize;
            initializeKeys(PianoKeys);
            initializekeyEnabledDictionary(keyEnabledDictionary);
            initializePianoSoundDictionary(pianoSounds);
            Note.initializeAlterationNotesDictionary();
            Note.initializeMusicalNoteNamesDictionary();
            Note.initializeMusicalOctaveDictionary();
            foreach (Key recentKeyPress in PianoKeys.Keys) //constructor for dictionary
            {
                startTimeOfNote.Add(recentKeyPress, null); 
            }          
        }
        
        public void initializePianoSoundDictionary(Dictionary<Key, int> dict)
        {
            dict.Add(Key.A, 60); //C4
            dict.Add(Key.W, 61); //C4#
            dict.Add(Key.S, 62); //D4
            dict.Add(Key.E, 63); //Eb4 or D4#
            dict.Add(Key.D, 64); //E4
            dict.Add(Key.F, 65); //F4
            dict.Add(Key.T, 66); //Gb4 or F4#
            dict.Add(Key.G, 67); //G4
            dict.Add(Key.Y, 68); //Ab4 or G4#
            dict.Add(Key.H, 69); //A4
            dict.Add(Key.U, 70); //Bb4 or A4#
            dict.Add(Key.J, 71); //B4
            dict.Add(Key.K, 72); //C5
            dict.Add(Key.O, 73); //Db5 or C5#
            dict.Add(Key.L, 74); //D5
            dict.Add(Key.P, 75); //Eb5 orD5#


        }

        public void initializekeyEnabledDictionary(Dictionary<Key, bool> dict)
        {
           dict.Add(Key.A, true);
           dict.Add(Key.S, true);
           dict.Add(Key.D, true);
           dict.Add(Key.F, true);
           dict.Add(Key.G, true);
           dict.Add(Key.H, true);
           dict.Add(Key.J, true);
           dict.Add(Key.K, true);
           dict.Add(Key.L, true);

            //Black keys
           dict.Add(Key.W, true);
           dict.Add(Key.E, true);
           dict.Add(Key.T, true); 
           dict.Add(Key.Y, true);
           dict.Add(Key.U, true);
           dict.Add(Key.O, true);
           dict.Add(Key.P, true);


        }

        public void initializeKeys(Dictionary<Key, Button> dict)
        {
            dict.Add(Key.A, C4_NaturalNote);
            dict.Add(Key.S, D4_NaturalNote);
            dict.Add(Key.D, E4_NaturalNote);
            dict.Add(Key.F, F4_NaturalNote);
            dict.Add(Key.G, G4_NaturalNote);
            dict.Add(Key.H, A4_NaturalNote);
            dict.Add(Key.J, B4_NaturalNote);
            dict.Add(Key.K, C5_NaturalNote);
            dict.Add(Key.L, D5_NaturalNote);

            //Black keys
            dict.Add(Key.W, C4_SharpNote);
            dict.Add(Key.E, D4_SharpNote);
            dict.Add(Key.T, F4_SharpNote);
            dict.Add(Key.Y, G4_SharpNote);
            dict.Add(Key.U, A4_SharpNote);
            dict.Add(Key.O, C5_SharpNote);
            dict.Add(Key.P, D5_SharpNote);
            
        }


        //keeps window from changing
        private void Window_SizeChanged(object sender, SizeChangedEventArgs e) 
        {
            return;
        }
       
        private void Grid_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (keyEnabledDictionary[e.Key])
                {
                    //set to true to stop method from constantly firing
                    keyEnabledDictionary[e.Key] = false;//make dictionary-for style
                    //changes button to red and border black
                    Button keyPressed = PianoKeys[e.Key];
                    
                    keyPressed.Background = Brushes.Red;
                    keyPressed.BorderBrush = Brushes.Black;

                    int pianoSound = pianoSounds[e.Key];
                    midiOut.Send(MidiMessage.StartNote(pianoSound, 127, channel).RawData);
                    



                    //get time of key press event
                    DateTime timeOfKeyDownPress = DateTime.Now;


                    //store the value of the moment the key was pressed to the associated key
                    startTimeOfNote[e.Key] = timeOfKeyDownPress;
                    
                }
   

            }
            catch(KeyNotFoundException) //maybe put message
            { 
            }
        }

        private void Grid_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                //set index of e.Key back to true so keydown event can occur
                keyEnabledDictionary[e.Key] = true;
                
                //changes button back to tranparent
                Button keyPressed = PianoKeys[e.Key];
                

                
                keyPressed.Background = Brushes.Transparent;
                keyPressed.BorderBrush = Brushes.Transparent;

                
                int pianoSound = pianoSounds[e.Key];
                midiOut.Send(MidiMessage.StopNote(pianoSound,0,channel).RawData);

                //decides whether note played is a single note or a chord
                DateTime timeOfKeyUpPress = DateTime.Now;

                //type cast because value in dictionary is no longer null-this gets initial key press
                DateTime timeOfKeyDownPress = (DateTime)startTimeOfNote[e.Key];

                //method sorts dictionary and adds notes to chord if they are pressed at same time
                timeOfKeyDownPress = sortPlayedNotes(playedNotes, timeOfKeyDownPress);

                

                //create instance object note
                Note currentNotePlaying = new Note(e.Key, timeOfKeyDownPress, timeOfKeyUpPress);
               
                if(playedNotes.ContainsKey(timeOfKeyDownPress))
                {
                    SortedList<Note, Note> chord = playedNotes[timeOfKeyDownPress];
                    //sorted into correct weight with descending order
                    chord.Add(currentNotePlaying, currentNotePlaying);
                    
                }
                //else store in dictionary as single note
                else
                {
                    //create new list and add Note to list
                    SortedList<Note, Note> singleNote = new SortedList<Note,Note>();
                    singleNote.Add(currentNotePlaying, currentNotePlaying);

                    //add list to dictionary
                    playedNotes.Add(timeOfKeyDownPress, singleNote);
                }
            
            }
            catch (KeyNotFoundException) { }           
        }

        private DateTime sortPlayedNotes(Dictionary<DateTime, SortedList<Note, Note>> playedNotes, DateTime timeOfKeyDownPress)
        {
            //range of 18ms
            TimeSpan rangeForChord = new TimeSpan(0, 0, 0, 0, 18);
            
            List<DateTime> sortedPlayedNotesByKeyDown = playedNotes.Keys.ToList();
            
            //uses quicksort so time-complexity = N log(N))-average, worst case n^2
            sortedPlayedNotesByKeyDown.Sort();

            //loop through sorted list to see if notes should be grouped together by their 
            //key down event
            //loop is N, total complexity of method is N^2 + N or Big-O is O(n^2)
            foreach (DateTime keydown in sortedPlayedNotesByKeyDown)
            {
                if (rangeForChord.TotalMilliseconds > Math.Abs((timeOfKeyDownPress - keydown).TotalMilliseconds))
                {
                    
                    timeOfKeyDownPress = keydown;

                }
            }
            return timeOfKeyDownPress;

        }

        private void record_Button(object sender, RoutedEventArgs e)
        {
            if (recordOn)
            {
                recordButton.Content = "Stop";
                recordOn = false;
                playedNotes.Clear();
                
            }
            else
            {
                recordButton.Content = "Record";
                recordOn = true;
                adjustMeasures();
                MusicXMLWriter.writeMusic(playedNotes);
                
            }
        }

        private void adjustMeasures()
        {
            int totalWeightOfMeasure = 64;

            int noteWeightCounter = 0;
            TimeSpan range = new TimeSpan(0, 0, 0, 0, 1); //used to create datetime to store in playedNotes dictionary


            //collection to modify notes in playedNote dictionary, use this dictionary to reassign playedNotes dictionary
            Dictionary<DateTime, SortedList<Note, Note>> modifiedNotes = new Dictionary<DateTime, SortedList<Note, Note>>();

            foreach (SortedList<Note, Note> listOfNotes in playedNotes.Values)
            {
                
                if (noteWeightCounter == totalWeightOfMeasure)
                {
                    noteWeightCounter = 0;
                }

                if (listOfNotes.Count == 1)
                {

                    int baseOfPower = 2;
                    Note currentNote = listOfNotes.Keys[0];
                    int singleWeightOfNote = currentNote.weight;
                    //find value to get full measure
                    int valueForCompleteMeasure = totalWeightOfMeasure - noteWeightCounter;
                    noteWeightCounter += singleWeightOfNote;


                    if (noteWeightCounter > totalWeightOfMeasure && noteWeightCounter - singleWeightOfNote < totalWeightOfMeasure)
                    {
                       
                        int sumOfTiedNoteWeights = 0;
                        int counter = 1;

                        while (true)
                        {
                            
                            
                            int powerOfTwo = (int)(Math.Log(valueForCompleteMeasure-sumOfTiedNoteWeights, baseOfPower));

                            Note tiedNote = calculateTiedNotes(currentNote, powerOfTwo, range, counter);

                           
                            //add tiedNote to dictionary
                            SortedList<Note, Note> tiedNoteList1 = new SortedList<Note, Note>();
                            tiedNoteList1.Add(tiedNote, tiedNote);
                            modifiedNotes.Add(tiedNote.startOfKeyPress, tiedNoteList1);
                            sumOfTiedNoteWeights += tiedNote.weight;
                            
                            if(sumOfTiedNoteWeights == valueForCompleteMeasure)
                            {
                                counter++;
                                break;
                            }
                            counter++;
                        }

                        
                        //reset variables
                        noteWeightCounter = 0;
                        sumOfTiedNoteWeights = 0;
                 
                        int secondMeasureWeight = currentNote.weight - valueForCompleteMeasure;
                        
                        while (true)
                        {
                            int logOfValueForCompleteMeasure = (int)Math.Log(secondMeasureWeight - sumOfTiedNoteWeights, baseOfPower);

                            Note secondPartOfTiedNote = calculateTiedNotes(currentNote, logOfValueForCompleteMeasure, range, counter);

                            sumOfTiedNoteWeights += secondPartOfTiedNote.weight;
                            if(sumOfTiedNoteWeights == secondMeasureWeight)
                            {
                                int end = 2;
                                secondPartOfTiedNote.setTied(end);
                                SortedList<Note, Note> tiedNoteListLastNote = new SortedList<Note, Note>();
                                tiedNoteListLastNote.Add(secondPartOfTiedNote, secondPartOfTiedNote);

                                //add secondPartOfTiedNote to modifiedNote Dictionary
                                modifiedNotes.Add(secondPartOfTiedNote.startOfKeyPress, tiedNoteListLastNote);

                                //noteweight counter
                                noteWeightCounter += secondPartOfTiedNote.weight;
                                break;
                            }

                            SortedList<Note, Note> tiedNoteList2 = new SortedList<Note, Note>();
                            tiedNoteList2.Add(secondPartOfTiedNote, secondPartOfTiedNote);

                            //add secondPartOfTiedNote to modifiedNote Dictionary
                            modifiedNotes.Add(secondPartOfTiedNote.startOfKeyPress, tiedNoteList2);

                            //reset noteweight counter
                            noteWeightCounter += secondPartOfTiedNote.weight;
                            counter++;

                        }

                    }
                    else
                    {
                        Note unchangedNote = new Note(currentNote.pitch, currentNote.startOfKeyPress, currentNote.endOfKeyPress);

                        SortedList<Note, Note> unchangedNoteList1 = new SortedList<Note, Note>();
                        unchangedNoteList1.Add(unchangedNote, unchangedNote);
                        modifiedNotes.Add(unchangedNote.startOfKeyPress, unchangedNoteList1);


                    }
                }
                //otherwise we have a chord
                else
                {
                    for(int i = 0; i < listOfNotes.Count; i++) 
                    {
                        Note currentNote = listOfNotes.Keys[i];
                        if(i == 0)
                        {
                            int singleWeightOfNote = currentNote.weight;
                            noteWeightCounter += singleWeightOfNote;
                            if(noteWeightCounter > totalWeightOfMeasure)
                            {
                                noteWeightCounter = singleWeightOfNote;
                            }
                            Note unchangedNote = new Note(currentNote.pitch, currentNote.startOfKeyPress, currentNote.endOfKeyPress);

                            SortedList<Note, Note> unchangedNoteList1 = new SortedList<Note, Note>();
                            unchangedNoteList1.Add(unchangedNote, unchangedNote);
                            modifiedNotes.Add(unchangedNote.startOfKeyPress, unchangedNoteList1);
                        }
                        else
                        {
                            Note unchangedNote = new Note(currentNote.pitch, currentNote.startOfKeyPress, currentNote.endOfKeyPress);
                           
                            SortedList<Note, Note> chord = modifiedNotes[currentNote.startOfKeyPress];
                            chord.Add(unchangedNote, unchangedNote);
                            
                        }
                    }
                }
            }
            //reassignment
            playedNotes = modifiedNotes;
        }

        public Note calculateTiedNotes(Note currentNote, int powerOfTwo, TimeSpan range, int counter) 
        {
            int baseOfPower = 2;
            //make a deep copy of current note
            Note tiedNote = new Note(currentNote.pitch, currentNote.startOfKeyPress, currentNote.endOfKeyPress);

            tiedNote.weight = (int)Math.Pow(baseOfPower, powerOfTwo);
            int start = 1;
            tiedNote.setTied(start);


            tiedNote.setTypeOfNoteBasedOnWeight();

            range = TimeSpan.FromTicks(range.Ticks * counter);

            tiedNote.startOfKeyPress += range;

            return tiedNote;
        }


        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            midiOut.Close();
            midiOut.Dispose();

        }

        private void Synth_Strings_Click(object sender, RoutedEventArgs e)
        {
            int voice = 50;
            midiOut.Send(MidiMessage.ChangePatch(voice, channel).RawData);
            Acoustic_Piano.Background = Brushes.Gray;
            Acoustic_Piano.BorderBrush = Brushes.Gray;
            Synth_Strings.Background = Brushes.Red;
            Synth_Strings.BorderBrush = Brushes.Red;
        }

        private void Acoustic_Piano_Click(object sender, RoutedEventArgs e)
        {
            int voice = 0;
            midiOut.Send(MidiMessage.ChangePatch(voice, channel).RawData);
            Acoustic_Piano.Background = Brushes.Red;
            Acoustic_Piano.BorderBrush = Brushes.Red;
            Synth_Strings.Background = Brushes.Gray;
            Synth_Strings.BorderBrush = Brushes.Gray;

        }
    }
}
