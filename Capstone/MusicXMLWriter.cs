using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Xml.Linq;

namespace Capstone
{
    internal class MusicXMLWriter
    {
        //file path for xml file 
        const string xmlfile = "C:/Users/14145/OneDrive/Desktop/Capstone/ScriptedMusicXML/ScriptedMusicXMLDoc.xml";
       
        public static void writeMusic(Dictionary<DateTime, SortedList<Note,Note>> playedNotes)
        {
            string ID = "P1";
            string time = "4";
            int totalWeightOfMeasure = 64;
            CreateXMLFile();
            XDocument xdoc = XDocument.Load(xmlfile);
            AddXMLPartListTitle(xdoc,ID);
            
            XElement part = new XElement("part");
            XAttribute partid = new XAttribute("id", ID);
            xdoc.Root.Add(part);
            part.Add(partid);

            //only need temp and beats in first measure
            XElement tempo = new XElement("time");
            XElement beats = new XElement("beats", time);
            XElement beatType = new XElement("beat-type", time);
            tempo.Add(beats);
            tempo.Add(beatType);
            
            //create measure and measure 1 is the only measure that needs temp and time
            XElement measure = new XElement("measure");

            //counts number of measures in song
            int measureCounter = 1;
            XAttribute measureNumber = new XAttribute("number", measureCounter.ToString());
            measure.Add(measureNumber);
            XElement measureAttributes = new XElement("attributes");
            measureAttributes.Add(tempo);
            measure.Add(measureAttributes);
            part.Add(measure);

            //track weight of notes inside measure
            int noteWeightCounter = 0;


            foreach (SortedList<Note,Note> listOfNotes in playedNotes.Values) 
            {     
                XElement note = new XElement("note");
                XElement pitch = new XElement("pitch");
                XElement type =  new XElement("type");
                XElement voice = new XElement("voice");

                for (int i = 0; i < listOfNotes.Count; i++)
                {
                    Note currentnote = listOfNotes.Keys[i];

                    //more than 1 note in list = chord
                    if (listOfNotes.Count >= 2)
                    {
                        if (i >= 1)
                        {
                            XElement chord = new XElement("chord");
                            //call overloaded method
                            measure.Add(ScriptNote(currentnote, chord));
                        }
                        //first iteration of of looping through chord
                        else
                        {
                            
                            //reassign current note after we sorted listOfNotes
                            currentnote = listOfNotes.Keys[i];
                            XAttribute noteDefaultXValue = new XAttribute("default-x", "100");
                            XElement beginningChordNote = (ScriptNote(currentnote));
                            beginningChordNote.Add(noteDefaultXValue);
                            
                            //puts notes in the correct measure
                            int singleWeightOfNote = currentnote.weight;
                            noteWeightCounter += singleWeightOfNote;

                            if (noteWeightCounter > totalWeightOfMeasure)
                            {
                                measureCounter++;
                                noteWeightCounter = singleWeightOfNote;
                                measure = new XElement("measure");
                                measureNumber = new XAttribute("number", measureCounter.ToString());
                                measure.Add(measureNumber);
                                part.Add(measure);
                            }

                            measure.Add(beginningChordNote);
                        }

                    }
                    
                    //sortedlistOfNotes is only one note
                    else
                    {
                        //current note is not a tied note
                        if (currentnote.getTied() == 0)
                        {
                            //puts notes in the correct measure
                            int singleWeightOfNote = currentnote.weight;
                            noteWeightCounter += singleWeightOfNote;

                            if (noteWeightCounter > totalWeightOfMeasure)
                            {
                                measureCounter++;
                                noteWeightCounter = singleWeightOfNote;
                                measure = new XElement("measure");
                                measureNumber = new XAttribute("number", measureCounter.ToString());
                                measure.Add(measureNumber);
                                part.Add(measure);
                            }

                            measure.Add(ScriptNote(currentnote));
                        }
                        //current note is a tied note
                        else 
                        {
                            //start of tied note
                            if(currentnote.getTied() == 1)
                            {
                                XElement startOfTiedNote = beginningOfTiedNote(currentnote);
                                int singleWeightOfNote = currentnote.weight;
                                noteWeightCounter += singleWeightOfNote;

                                if (noteWeightCounter > totalWeightOfMeasure)
                                {
                                    measureCounter++;
                                    noteWeightCounter = singleWeightOfNote;
                                    measure = new XElement("measure");
                                    measureNumber = new XAttribute("number", measureCounter.ToString());
                                    measure.Add(measureNumber);
                                    part.Add(measure);
                                }

                                measure.Add(startOfTiedNote);

                            }
                            //end of tied note
                            else 
                            {
                                XElement endOfTiedNote = stopTiedNote(currentnote);
                                int singleWeightOfNote = currentnote.weight;
                                noteWeightCounter += singleWeightOfNote;

                                if (noteWeightCounter > totalWeightOfMeasure)
                                {
                                    measureCounter++;
                                    noteWeightCounter = singleWeightOfNote;
                                    measure = new XElement("measure");
                                    measureNumber = new XAttribute("number", measureCounter.ToString());
                                    measure.Add(measureNumber);
                                    part.Add(measure);
                                }

                                measure.Add(endOfTiedNote);
                            }
                        }
                    }

                }


            }
            xdoc.Save(xmlfile);
        }

        //method for start of tied note
        public static XElement beginningOfTiedNote(Note currentnote)
        {
            XElement note = new XElement("note");
            XElement pitch = new XElement("pitch");
            XElement duration = new XElement("duration", currentnote.weight.ToString());
            //step for tied notes
            XElement tie = new XElement("tie");
            XAttribute startTie = new XAttribute("type", "start");
            tie.Add(startTie);
            XElement type = new XElement("type", currentnote.getTypeOfNoteAsString());
            XElement voice = new XElement("voice");
            XElement step = new XElement("step", currentnote.getNameOfNote());
            //this is the step for tied notes
            XElement notations = new XElement("notations");
            XElement tied = new XElement("tied");
            XAttribute tiedStart = new XAttribute("type", "start");
            tied.Add(tiedStart);
            notations.Add(tied);
            
            pitch.Add(step);
            if (currentnote.getAlterationOfNote() != null)
            {
                XElement alter = new XElement("alter", currentnote.getAlterationOfNote());
                pitch.Add(alter);
            }
            XElement octave = new XElement("octave", currentnote.getOctave());
            pitch.Add(octave);
            voice = new XElement("voice", 1);


            note.Add(pitch);
            note.Add(duration);
            note.Add(tie); 
            note.Add(voice);
            note.Add(type);
            note.Add(notations); //for tied notes

            return note;
        }
        public static XElement stopTiedNote(Note currentnote)
        {
            XElement note = new XElement("note");
            XElement pitch = new XElement("pitch");
            XElement duration = new XElement("duration", currentnote.weight.ToString());

            //this step stops the tie
            XElement tie = new XElement("tie");
            XAttribute stopTie = new XAttribute("type", "stop");
            tie.Add(stopTie);
            
            XElement type = new XElement("type", currentnote.getTypeOfNoteAsString());
            XElement voice = new XElement("voice");
            XElement step = new XElement("step", currentnote.getNameOfNote());
            
            //this is the step that stops the tie
            XElement notations = new XElement("notations");
            XElement tied = new XElement("tied");
            XAttribute tiedEnd = new XAttribute("type", "stop");
            tied.Add(tiedEnd);
            notations.Add(tied);
            
            pitch.Add(step);
            if (currentnote.getAlterationOfNote() != null)
            {
                XElement alter = new XElement("alter", currentnote.getAlterationOfNote());
                pitch.Add(alter);
            }
            XElement octave = new XElement("octave", currentnote.getOctave());
            pitch.Add(octave);
            voice = new XElement("voice", 1);


            note.Add(pitch);
            note.Add(duration);
            note.Add(tie);
            note.Add(voice);
            note.Add(type);
            note.Add(notations);//stops tie

            return note;

        }


        //overloaded method
        public static XElement ScriptNote(Note currentnote, XElement chord)
        {
            
            XElement note = new XElement("note");
            XAttribute noteDefault = new XAttribute("default-x", "100");
            note.Add(noteDefault);
            XElement pitch = new XElement("pitch");
            XElement duration = new XElement("duration", currentnote.weight.ToString());  
            XElement type = new XElement("type", currentnote.getTypeOfNoteAsString());
            XElement voice = new XElement("voice");
            XElement step = new XElement("step", currentnote.getNameOfNote());
            pitch.Add(step);
            if (currentnote.getAlterationOfNote() != null)
            {
                XElement alter = new XElement("alter", currentnote.getAlterationOfNote());
                pitch.Add(alter);
            }
            XElement octave = new XElement("octave", currentnote.getOctave());
            pitch.Add(octave);
            voice = new XElement("voice", 1);
            

            note.Add(chord);
            note.Add(pitch);
            note.Add(duration);
            note.Add(voice);
            note.Add(type);
            

            return note;
        }
        public static XElement ScriptNote(Note currentnote)
        {
            XElement note = new XElement("note");
            XElement pitch = new XElement("pitch");
            XElement duration = new XElement("duration", currentnote.weight.ToString());
            XElement type = new XElement("type", currentnote.getTypeOfNoteAsString());
            XElement voice = new XElement("voice");
            XElement step = new XElement("step", currentnote.getNameOfNote());
            pitch.Add(step);
            if (currentnote.getAlterationOfNote() != null)
            {
                XElement alter = new XElement("alter", currentnote.getAlterationOfNote());
                pitch.Add(alter);
            }
            XElement octave = new XElement("octave", currentnote.getOctave());
            pitch.Add(octave);
            voice = new XElement("voice", 1);
            

            note.Add(pitch);
            note.Add(duration);
            note.Add(voice);
            note.Add(type);
            
            

            return note;
        }


        public static void CreateXMLFile()
        {
            //need to check if file already exists if it does then delete it and 
            //create new xml doc.
            if(File.Exists(xmlfile))
            {
                File.Delete(xmlfile);
            }
            XDocument xdoc = new XDocument(new XDeclaration("1.0", "utf-8", "no"),
                new XElement("score-partwise"));
            xdoc.Save(xmlfile);
        }

        public static void AddXMLPartListTitle(XDocument xdoc, string ID)
        {           
            XElement partList = new XElement("part-list"); //part-list requered in all MusicXML 
            XElement scorePart = new XElement("score-part");
            XAttribute scorePartID = new XAttribute("id", ID);
            scorePart.Add(scorePartID);
            XElement partName = new XElement("part-name", "piano");
            scorePart.Add(partName);
            partList.Add(scorePart);
            xdoc.Root.Add(partList);
            

        }
    }
}
