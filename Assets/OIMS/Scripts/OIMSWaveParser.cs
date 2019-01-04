using System.Collections;
using System.Collections.Generic;
using System.IO;
using System;
using UnityEngine;

public class OIMSWaveParser {

    bool debugMode = false;

    struct RiffHeader
    {   
        public byte[] chunkID; // "riff" - Marks the file as a riff file. Characters are each 1 byte long.
        public uint size; // Size of the overall file – 8 bytes, in bytes (32-bit integer). Typically, you’d fill this in after creation.

    }

	struct WavHeader
    {
        public byte[] chunkID; // "riff" - Marks the file as a riff file. Characters are each 1 byte long.
        public uint size; // Size of the overall file – 8 bytes, in bytes (32-bit integer). Typically, you’d fill this in after creation.
        public byte[] typeID;  // "WAVE" - 	File Type Header. For our purposes, it always equals “WAVE”.
        public byte[] fmtID;  // "fmt " - Format chunk marker. Includes trailing null
        public uint fmtSize; // Length of format data as listed above - 16
        public ushort format; // Type of format (1 is PCM) – 2 byte integer
        public ushort channels; // Number of Channels – 2 byte integer
        public uint sampleRate; // Sample Rate – 32 byte integer. Common values are 44100 (CD), 48000 (DAT). Sample Rate = Number of Samples per second, or Hertz.
        public uint bytePerSec; // (Sample Rate * BitsPerSample * Channels) / 8.
        public ushort blockSize; // (BitsPerSample * Channels) / 8.1 – 8 bit mono2 – 8 bit stereo/16 bit mono4 – 16 bit stereo
        public ushort bit;  // Bits per sample
        public byte[] dataID; // “data” chunk header. Marks the beginning of the data section.
        public uint dataSize; // Size of the data section.
    }

    struct WavData
    {



    }

    struct CueHeader
    {
        public byte[] chunkID; //this is 'cue '
        public uint size; //this is the chunk data size - ChunkDataSize = 4 + (NumCuePoints * 24)
        public uint numberOfCuePoints; //this is the number of cue points in the cue chunk

    }

    struct CuePoint
    {
        public uint dataID;
        public uint position; 
        public uint dataChunkID; 
        public uint chunkStart; 
        public uint blockStart;
        public uint sampleOffset;

        // 4 + 4 + 4 + 4 + 4 + 4
    }

    struct ListHeader
    {
        public byte[] chunkID; //this is 'list'
        public uint size; //this is the chunk data size - 
        public byte[] typeID; //this is 'adtl'


    }

    struct LabelChunk
    {
        public byte[] chunkID; //this is 'labl'
        public uint size; //this is the chunk data size - 
        public uint cuePointID; //0 - 0xFFFFFFFF
        public byte[] text; 
    }


	public void LoadWaveData(string assetPath, out float startTime, out float endTime)
	{
		WavHeader theHeader = new WavHeader();
        CueHeader theCueHeader = new CueHeader();
        //CuePoint theCuePoint = new CuePoint();
        ListHeader theListHeader = new ListHeader();
        //LabelChunk theLabelChunk = new LabelChunk();

        ArrayList markerTimeList = new ArrayList();

        //List<short> lDataList = new List<short>();
        //List<short> rDataList = new List<short>();

        startTime = -1.0f;
        endTime = -1.0f;

        uint riffProgress = 0;

		//using (FileStream fs = new FileStream("/Users/msweet/Desktop/Chimes_with_marker_24.wav", System.IO.FileMode.Open, System.IO.FileAccess.Read))
        using (FileStream fs = new FileStream(assetPath, System.IO.FileMode.Open, System.IO.FileAccess.Read))
            using (BinaryReader br = new BinaryReader(fs))
            {
                try
                {
                    OIMSDebug("Trying to load the WAV data: " + assetPath);
					theHeader.chunkID = br.ReadBytes(4); //riff
                    riffProgress+=4;
                    theHeader.size = br.ReadUInt32(); //size of the total file!
                    riffProgress+=4;
                    theHeader.typeID = br.ReadBytes(4); //wave or something else
                    riffProgress+=4;
                    //if the file is not riff and wave - then we should exit here

                    if(System.Text.Encoding.ASCII.GetString(theHeader.chunkID) != "RIFF"){
                        OIMSDebug("The file is not RIFF encoded!! " + System.Text.Encoding.ASCII.GetString(theHeader.chunkID) + "    " + assetPath);
                        return;
                    }

                    if(System.Text.Encoding.ASCII.GetString(theHeader.typeID) != "WAVE"){
                        OIMSDebug("The file is not WAVE encoded!! " + System.Text.Encoding.ASCII.GetString(theHeader.typeID) + "    " + assetPath);
                        return;
                    }

                    OIMSDebug("We have a wave file - continuing on to parse the wave data.");

                    //next we need to parse the wave data

                    theHeader.fmtID = br.ReadBytes(4);  //should equal 'fmt ' with the trailing space  
                    riffProgress+=4;
                    theHeader.fmtSize = br.ReadUInt32(); //not sure what this should be? but equals 16 - remaining chunk length after header?
                    riffProgress+=4;
                    theHeader.format = br.ReadUInt16(); //not sure what this is? but equals 1?  i think this defines pcm or adpcm
                    riffProgress+=2;
                    theHeader.channels = br.ReadUInt16(); //1 for mono, 2 for stereo etc.
                    riffProgress+=2;
                    theHeader.sampleRate = br.ReadUInt32(); //22050, 44100, 48000, etc.
                    riffProgress+=4;
                    theHeader.bytePerSec = br.ReadUInt32(); //Bytes per second = (Sample Rate * BitsPerSample * Channels) / 8.
                    riffProgress+=4;
                    theHeader.blockSize = br.ReadUInt16(); //Block align = (BitsPerSample * Channels) / 8   this is 1 if 8 bit mono, this is 6 if 24 bit stereo, this is 2 if 16 bit mono
                    riffProgress+=2;
                    theHeader.bit = br.ReadUInt16(); //bit rate e.g. 8, 16, 24
                    riffProgress+=2;
                    theHeader.dataID = br.ReadBytes(4); //this should be 'data'
                    riffProgress+=4;
                    theHeader.dataSize = br.ReadUInt32(); //this is how much data we have
                    riffProgress+=4;

                    // OIMSDebug("theHeader.typeID: " + System.Text.Encoding.ASCII.GetString(theHeader.typeID));
					// OIMSDebug("theHeader.fmtID: " + System.Text.Encoding.ASCII.GetString(theHeader.fmtID));
					// OIMSDebug("theHeader.fmtSize: " + theHeader.fmtSize);
					// OIMSDebug("theHeader.format: " + theHeader.format);
					// OIMSDebug("theHeader.channels: " + theHeader.channels);
					// OIMSDebug("theHeader.sampleRate: " + theHeader.sampleRate);
					// OIMSDebug("theHeader.bytePerSec: " + theHeader.bytePerSec);
					// OIMSDebug("theHeader.blockSize: " + theHeader.blockSize);
					// OIMSDebug("theHeader.bit: " + theHeader.bit); 
                    // OIMSDebug("theHeader.dataID: " + System.Text.Encoding.ASCII.GetString(theHeader.dataID));
                    // OIMSDebug("theHeader.dataSize: " + theHeader.dataSize);

					for (int i = 0; i < theHeader.dataSize / theHeader.blockSize; i++)
                    {
                        //byte[] x = br.ReadBytes(theHeader.blockSize);
                        br.ReadBytes(theHeader.blockSize);
                        riffProgress = riffProgress + theHeader.blockSize;
                        //lDataList.Add((short)br.ReadUInt16());
                        //rDataList.Add((short)br.ReadUInt16());
					 	//OIMSDebug("i: " + theHeader.blockSize + "    " + i);
                    }

                    
                    // THEN THINGS GET WEIRD - it could be any type of chunk next, and we have to look at then one at a time

                    bool done = false;

                    while(!done && riffProgress <= theHeader.size)
                    {
                        OIMSDebug("riffProgress: " + riffProgress + "  out of the full file size:" + theHeader.size + " = " + (theHeader.size - riffProgress) + " left.");
                        
                        //lets look at the next chunk
                        byte[] chunkID = br.ReadBytes(4);
                        riffProgress+=4;

                        OIMSDebug("The chunk id is: " + System.Text.Encoding.ASCII.GetString(chunkID));

                        bool chunkThatWeRecognize = false;

                        OIMSDebug("System.Text.Encoding.ASCII.GetString(chunkID).Length = " + System.Text.Encoding.ASCII.GetString(chunkID).Length);



                        //what is it? 'bext' or 'cue ' or something else
                        if(System.Text.Encoding.ASCII.GetString(chunkID).Substring(0, 3) == "cue")
                        {
                            //yay it's a cue!  I know how to do that!
                            OIMSDebug("Found cue chunk!");
                            theCueHeader.size = br.ReadUInt32(); //the size
                            riffProgress+=4;
                            theCueHeader.numberOfCuePoints = br.ReadUInt32(); //number of cue points
                            riffProgress+=4;

					        //OIMSDebug("1theCueHeader.chunkID: " + System.Text.Encoding.ASCII.GetString(theCueHeader.chunkID));
                            OIMSDebug("1theCueHeader.size: " + theCueHeader.size);
                            OIMSDebug("1theCueHeader.numberOfCuePoints: " + theCueHeader.numberOfCuePoints); 
                    
                            //next iterate through the cue points 

                            for(int i = 0; i < theCueHeader.numberOfCuePoints; i++ )
                            {
                                CuePoint thisCuePoint = new CuePoint();

                                thisCuePoint.dataID = br.ReadUInt32();
                                riffProgress+=4;
                                thisCuePoint.position = br.ReadUInt32();
                                riffProgress+=4;
                                thisCuePoint.dataChunkID = br.ReadUInt32();
                                riffProgress+=4;
                                thisCuePoint.chunkStart = br.ReadUInt32();
                                riffProgress+=4;
                                thisCuePoint.blockStart = br.ReadUInt32();
                                riffProgress+=4;
                                thisCuePoint.sampleOffset = br.ReadUInt32();
                                riffProgress+=4;
                                OIMSDebug("thisCuePoint.sampleOffset: " + thisCuePoint.sampleOffset);
                                float sampleOffset = (float)thisCuePoint.sampleOffset / (float)theHeader.sampleRate * 1000.0f;
                                OIMSDebug("thisCuePoint.sampleOffset in msec is: " + sampleOffset);
                                markerTimeList.Add(sampleOffset);
                            }
                            chunkThatWeRecognize = true;
                        }

                        if(System.Text.Encoding.ASCII.GetString(chunkID) == "LIST")
                        {
                            OIMSDebug("Found list chunk!");
                            theListHeader.size = br.ReadUInt32(); //the size
                            riffProgress+=4;
                            theListHeader.typeID = br.ReadBytes(4); //should be 'adlt'
                            riffProgress+=4;

                            //then we need to iterate through the data
                
                            int j = 0;
                            int k = 0;
                            while(j < theListHeader.size)
                            {
                                LabelChunk thisLabelChunk = new LabelChunk();
                                thisLabelChunk.chunkID = br.ReadBytes(4); //should be 'labl'
                                riffProgress+=4;
                                OIMSDebug("thisLabelChunk.chunkID: " + System.Text.Encoding.ASCII.GetString(thisLabelChunk.chunkID));
                                j+=4;
                                thisLabelChunk.size = br.ReadUInt32(); //the size
                                riffProgress+=4;
                                j+=4;
                                thisLabelChunk.cuePointID = br.ReadUInt32();
                                riffProgress+=4;
                                j+=4; 
                                
                                thisLabelChunk.text = br.ReadBytes((int)thisLabelChunk.size - 4);
                                riffProgress = riffProgress + thisLabelChunk.size - 4;


                                OIMSDebug("thisLabelChunk.text: " + System.Text.Encoding.ASCII.GetString(thisLabelChunk.text));

                                string tempString = System.Text.Encoding.ASCII.GetString(thisLabelChunk.text).Trim().ToLower();
                    
                                OIMSDebug("tempString: " + tempString);
                                OIMSDebug("the length of tempString is: " + tempString.Length);

                                if(tempString.Length > 5)
                                {
                                    if(String.Equals(tempString.Substring(0, 5), "start"))
                                    {
                                        OIMSDebug("START TIME RETRIEVED!");
                                        if(markerTimeList[k] != null)
                                        {
                                            startTime = (float)markerTimeList[k];
                                        }
                                    }
                                }
                        
                                if(tempString.Length > 3)
                                {
                                    if(String.Equals(tempString.Substring(0, 3), "end"))
                                    {
                                        OIMSDebug("END TIME RETRIEVED!");
                                        if(markerTimeList[k] != null)
                                        {
                                            endTime = (float)markerTimeList[k];
                                        }
                                    }
                                
                                }
                        
                        
                                j+=(int)thisLabelChunk.size;
                                k++;
                            }

                            chunkThatWeRecognize = true;

                        }

                        if(System.Text.Encoding.ASCII.GetString(chunkID) == "bext")
                        {
                            OIMSDebug("Found bext chunk!");
                            uint size = br.ReadUInt32();
                            riffProgress+=4;
                            for (int i = 0; i < size; i++)
                            {
                                //byte[] x = br.ReadBytes(1);
                                br.ReadBytes(1);
                                riffProgress+=1;
                            }   
                            chunkThatWeRecognize = true;
                        }

                        if(System.Text.Encoding.ASCII.GetString(chunkID) == "JUNK")
                        {
                            OIMSDebug("Found JUNK chunk!");
                            uint size = br.ReadUInt32();
                            riffProgress+=4;
                            for (int i = 0; i < size; i++)
                            {
                                //byte[] x = br.ReadBytes(1);
                                br.ReadBytes(1);
                                riffProgress+=1;
                            }   
                            chunkThatWeRecognize = true;
                        }

                        if(System.Text.Encoding.ASCII.GetString(chunkID) == "MLo2")
                        {
                            OIMSDebug("Found MLo2 chunk!");
                            uint size = br.ReadUInt32();
                            riffProgress+=4;
                            for (int i = 0; i < size; i++)
                            {
                                //byte[] x = br.ReadBytes(1);
                                br.ReadBytes(1);
                                riffProgress+=1;
                            }   
                            chunkThatWeRecognize = true;
                        }

                        if(System.Text.Encoding.ASCII.GetString(chunkID) == "acid")
                        {
                            OIMSDebug("Found acid chunk!");
                            uint size = br.ReadUInt32();
                            riffProgress+=4;
                            for (int i = 0; i < size; i++)
                            {
                                //byte[] x = br.ReadBytes(1);
                                br.ReadBytes(1);
                                riffProgress+=1;
                            }   
                            chunkThatWeRecognize = true;
                        }

                        if(System.Text.Encoding.ASCII.GetString(chunkID) == "dpas")
                        {
                            OIMSDebug("Found dpas chunk!");
                            uint size = br.ReadUInt32();
                            riffProgress+=4;
                            for (int i = 0; i < size; i++)
                            {
                                //byte[] x = br.ReadBytes(1);
                                br.ReadBytes(1);
                                riffProgress+=1;
                            }   
                            chunkThatWeRecognize = true;
                        }

                        if(System.Text.Encoding.ASCII.GetString(chunkID) == "dprn")
                        {
                            OIMSDebug("Found dprn chunk!");
                            uint size = br.ReadUInt32();
                            riffProgress+=4;
                            for (int i = 0; i < size; i++)
                            {
                                //byte[] x = br.ReadBytes(1);
                                br.ReadBytes(1);
                                riffProgress+=1;
                            }   
                            chunkThatWeRecognize = true;
                        }

                        if(System.Text.Encoding.ASCII.GetString(chunkID) == "fact")
                        {
                            OIMSDebug("Found fact chunk!");
                            uint size = br.ReadUInt32();
                            riffProgress+=4;
                            for (int i = 0; i < size; i++)
                            {
                                //byte[] x = br.ReadBytes(1);
                                br.ReadBytes(1);
                                riffProgress+=1;
                            }   
                            chunkThatWeRecognize = true;
                        }



                        if(!chunkThatWeRecognize){
                            done = true;
                        }


                    }

                    OIMSDebug("FINAL riffProgress: " + riffProgress + "  out of the full file size:" + theHeader.size + " = " + (theHeader.size - riffProgress) + " left.");
                        


                }

				//Finally block is useful for running any code that must execute even if there is an exception. 
				//Control is passed to the Finally block regardless of how the Try ... Catch block exits

                finally
                {
                    if (br != null) //binary reader
                    {
                        br.Close();
                    }
                    if (fs != null) //file stream
                    {
                        fs.Close();
                    }
                }
            }

		OIMSDebug("WAV File opened.");

        

		//OIMSDebug();

		//Processing would happen here (passing for the time being)


	}

    public void LoadWavMarkers()
    {   








    }

    private void OIMSDebug(String whichString){
        if(debugMode)
        {
            OIMSDebug(whichString);
        }
    }




}

//if no markers, then no cue section
//if one marker, then cue, list, adtl, labl, marker 1, fact
//if two markers, then cue, list marker 1, labl, marker 2, fact
