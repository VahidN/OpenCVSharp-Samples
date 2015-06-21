/*
 ******************************************************************************
 * Evaluator.java
 * Module for evaluating object detection approaches on 
 * UIUC car image database (single-scale)
 *
 * Author: Shivani Agarwal (sagarwal@cs.uiuc.edu)
 *
 * Usage:
 * java Evaluator <trueLocationsFile> <foundLocationsFile> 
 *                [display <testImageDirectory> <displayDirectory>] 
 *
 * Updated: 14 July 2004
 ******************************************************************************
 */

import java.util.*;
import java.io.*;

public class Evaluator {

	public static int numTestImages = 170;

	public static int objHeight = 40;
	public static int objWidth = 100;

	public static double hAxisFraction = 0.25;
	public static double wAxisFraction = 0.25;

	public static int numObjects;
	public static int numPosDetections;
	public static int numFalseDetections;

	public static String testDir = "";
	public static String displayDir = "";

	public static void main(String[] args) {
		String trueLocFile = args[0];
		String foundLocFile = args[1];

		boolean display = false;

		if ((args.length > 2) && (args[2].equals("display"))) {
			if (args.length < 5) {
				System.err.println("\nIncorrect arguments\n");
				return;
			}
			else {
				display = true;
				testDir = args[3];
				displayDir = args[4];
			}
		}

		Vector[] trueLoc = getLocations(trueLocFile);
		Vector[] foundLoc = getLocations(foundLocFile);

		numObjects = count(trueLoc);

		float recall = evaluate(foundLoc, trueLoc, display);

		float precision = ((float)(numPosDetections))/
			((float)(numFalseDetections+numPosDetections));

		float fmeasure = 2 * recall * precision / (recall + precision);
	
		System.out.println("\nCorrect detections :  "+numPosDetections+
						   " out of "+numObjects+
						   "  (Recall: "+(100*recall)+" %)");
		System.out.println("False detections   :  "+numFalseDetections+
						   "           (Precision: "+
						   ((float)(100*precision))+" %)\n");
		System.out.println("F-measure :  " + (float)(100*fmeasure)+" %\n");
		
	}

	private static Vector[] getLocations(String locationFile) {
		Vector[] locations = new Vector[numTestImages];
		for (int n = 0; n < numTestImages; n++) {
			locations[n] = new Vector();
		}

		try {
			FileReader fr = new FileReader(locationFile);
			BufferedReader br = new BufferedReader(fr);
			StringTokenizer st;
			String token, nextLine;

			for (int n = 0; n < numTestImages; n++) {
				nextLine = br.readLine();
				st = new StringTokenizer(nextLine, " :(,)");
				token = st.nextToken();
				while (st.hasMoreTokens()) {
					token = st.nextToken();
					int i = Integer.parseInt(token);
					token = st.nextToken();
					int j = Integer.parseInt(token);
					Point p = new Point(i,j);
					locations[n].addElement(p);
				}
			}
			br.close();
		} catch(IOException e) {
			System.err.println("IO Exception");
		} return locations;
	}

	private static int count(Vector[] trueLoc) {
		int nObj = 0;

		for (int n = 0; n < numTestImages; n++) 
			for (int k = 0; k < trueLoc[n].size(); k++) 
				nObj++;

		return nObj;
	}

	private static float evaluate(Vector[] foundLoc, Vector[] trueLoc, 
								  boolean display) {
		float recall;

		try {
			double[][] image = new double[1][1];
			
			String statsFile = "break-up.txt";
			FileWriter fw = new FileWriter(statsFile);
			BufferedWriter bw = new BufferedWriter(fw);
	
			bw.write("\n----------------------------------------------\n");
			bw.write("Test     Number of    Correct       False \n");
			bw.write("Image    Objects      Detections    Detections \n"); 
			bw.write("----------------------------------------------\n\n");

			int numObjInImage;
			int falseDetInImage;
			
			for (int n = 0; n < numTestImages; n++) {
				Vector trueV = trueLoc[n];
				Vector foundV = foundLoc[n];
				numObjInImage = trueV.size();
				falseDetInImage = 0;
				if (display) 
					image = readImage(testDir+"/test-"+n+".pgm"); 
				for (int m = 0; m < foundV.size(); m++) {
					Point p = (Point) foundV.elementAt(m);
					boolean correct = false;
					int k = 0;				
					
					while ((! correct) && (k < trueV.size())) {
						Point q = (Point) trueV.elementAt(k);
						if (isAcceptable(p,q)) {
							correct = true;
							trueV.removeElementAt(k);
						}
						else k++;
					}
					
					if (correct) 
						numPosDetections++;
					else {
						numFalseDetections++;
						falseDetInImage++;
					}
					if (display) 
						markObject(image, p, correct);
				}
				if (display) 
					writeImage(image, displayDir+"/result-"+n+".pgm");
				
				bw.write("\t"+n+"\t\t\t"+numObjInImage+"\t\t\t"+
						 (numObjInImage-trueV.size())+"\t\t\t"+
						 falseDetInImage+"\n");

			}
			
			bw.write("\n------------------------------------------------\n");
			bw.write("\tTotal\t\t"+numObjects+"\t\t\t"+numPosDetections+
					 "\t\t\t"+numFalseDetections+"\n");
			bw.write("------------------------------------------------\n");
			bw.close();

		} catch(IOException e) {
			System.err.println("IO Exception");
		} 

		recall = (float)numPosDetections/(float)numObjects;
		return recall;
	}

		private static boolean isAcceptable(Point p, Point q) {
				boolean accept = false;

				int hDiff = Math.abs(p.i - q.i);
				int wDiff = Math.abs(p.j - q.j);

				double hAxis = hAxisFraction * objHeight;				
				double wAxis = wAxisFraction * objWidth;

				if ((hDiff*hDiff)/(hAxis*hAxis) + (wDiff*wDiff)/(wAxis*wAxis) <= 1) {
						accept = true;
				}

				return accept;	
		}


	private static void markObject(double[][] image, Point p, boolean correct){
		int height = image.length;
		int width = image[0].length;
		if ((p.i >= height) || (p.j >= width)) return;
		if ((p.i+objHeight < 0) || (p.j+objWidth < 0)) return;
		int i1 = p.i;
		int i2 = p.i + objHeight;
		int j1 = p.j;
		int j2 = p.j + objWidth;
		if (p.i < 0) i1 = 0;
		else if (p.i > (height-objHeight)) i2 = height-1;
		if (p.j < 0) j1 = 0;
		else if (p.j > (width-objWidth)) j2 = width-1; 

		if (correct) {
			if (p.j >= 0) {
				for (int i = i1; i < i2; i++) {
					image[i][p.j] = 255;
				}
			}
			if (p.j <= width-objWidth) {
				for (int i = i1; i < i2; i++) {
					image[i][p.j+objWidth-1] = 255;
				}
			}
			if (p.j+1 >= 0) {
				for (int i = i1+1; i < i2-1; i++) {
					image[i][p.j+1] = 0;
				}
			}
			if (p.j-1 <= width-objWidth) {
				for (int i = i1+1; i < i2-1; i++) {
					image[i][p.j+objWidth-2] = 0;
				}
			}
			if (p.j+2 >= 0) {
				for (int i = i1+2; i < i2-2; i++) {
					image[i][p.j+2] = 255;
				}
			}
			if (p.j-2 <= width-objWidth) {
				for (int i = i1+2; i < i2-2; i++) {
					image[i][p.j+objWidth-3] = 255;
				}
			}
			if (p.i >= 0) {
				for (int j = j1; j < j2; j++) {
					image[p.i][j] = 255;
				}
			}
			if (p.i <= height-objHeight) {
				for (int j = j1; j < j2; j++) {
					image[p.i+objHeight-1][j] = 255;
				}
			}
			if (p.i+1 >= 0) {
				for (int j = j1+1; j < j2-1; j++) {
					image[p.i+1][j] = 0;
				}
			}
			if (p.i-1 <= height-objHeight) {
				for (int j = j1+1; j < j2-1; j++) {
					image[p.i+objHeight-2][j] = 0;
				}
			}
			if (p.i+2 >= 0) {
				for (int j = j1+2; j < j2-2; j++) {
					image[p.i+2][j] = 255;
				}
			}
			if (p.i-2 <= height-objHeight) {
				for (int j = j1+2; j < j2-2; j++) {
					image[p.i+objHeight-3][j] = 255;
				}
			}
		}
		else {
			if (p.j >= 0) {
				for (int i = i1; i < i2; 
					 i += (i == p.i + objHeight/3) ? objHeight/3 : 1) {
					image[i][p.j] = 255;
				}
			}
			if (p.j <= width-objWidth) {
				for (int i = i1; i < i2; 
					 i += (i == p.i + objHeight/3) ? objHeight/3 : 1) {
					image[i][p.j+objWidth-1] = 255;
				}
			}
			if (p.j+1 >= 0) {
				for (int i = i1+1; i < i2-1; 
					 i += (i == p.i + objHeight/3) ? objHeight/3 : 1) {
					image[i][p.j+1] = 0;
				}
			}
			if (p.j-1 <= width-objWidth) {
				for (int i = i1+1; i < i2-1; 
					 i += (i == p.i + objHeight/3) ? objHeight/3 : 1) {
					image[i][p.j+objWidth-2] = 0;
				}
			}
			if (p.j+2 >= 0) {
				for (int i = i1+2; i < i2-2;
					 i += (i == p.i + objHeight/3) ? objHeight/3 : 1) {
					image[i][p.j+2] = 255;
				}
			}
			if (p.j-2 <= width-objWidth) {
				for (int i = i1+2; i < i2-2;
					 i += (i == p.i + objHeight/3) ? objHeight/3 : 1) {
					image[i][p.j+objWidth-3] = 255;
				}
			}
			if (p.i >= 0) {
				for (int j = j1; j < j2; 
					 j += (j == (p.j + objWidth/3) ? objWidth/3 : 1)) {
					image[p.i][j] = 255;
				}
			}
			if (p.i <= height-objHeight) {
				for (int j = j1; j < j2; 
					 j += (j == (p.j + objWidth/3) ? objWidth/3 : 1)) {
					image[p.i+objHeight-1][j] = 255;
				}
			}
			if (p.i+1 >= 0) {
				for (int j = j1+1; j < j2-1;
					 j += (j == (p.j + objWidth/3) ? objWidth/3 : 1)) {
					image[p.i+1][j] = 0;
				}
			}
			if (p.i-1 <= height-objHeight) {
				for (int j = j1+1; j < j2-1;
					 j += (j == (p.j + objWidth/3) ? objWidth/3 : 1)) {
					image[p.i+objHeight-2][j] = 0;
				}
			}
			if (p.i+2 >= 0) {
				for (int j = j1+2; j < j2-2; 
					 j += (j == (p.j + objWidth/3) ? objWidth/3 : 1)) {
					image[p.i+2][j] = 255;
				}
			}
			if (p.i-2 <= height-objHeight) {
				for (int j = j1+2; j < j2-2; 
					 j += (j == (p.j + objWidth/3) ? objWidth/3 : 1)) {
					image[p.i+objHeight-3][j] = 255;
				}
			}
		}

	}

	public static double[][] readImage(String imageFile) {
		double[][] image = null;
		try {
			FileReader fr = new FileReader(imageFile);
			BufferedReader br = new BufferedReader(fr);
			StringTokenizer st;
			String nextLine, token;
			int lineNo = 0;
			int i, j;
			i = j = 0;

			int width, height;
			width = height = 0;

			nextLine = br.readLine();
			st = new StringTokenizer(nextLine, " \n\t\r");
			token = st.nextToken();
			if (token.equals("P5"))
				lineNo++;
			else {
				System.err.println(imageFile + ": Invalid File Format");
				return null;
			}
			while (lineNo < 2) {
				nextLine = br.readLine();
				st = new StringTokenizer(nextLine, "# \n\t\r", true);
				if (nextLine.length() != 0) {
					token = st.nextToken();
					if (!(token.equals("#"))) {
						st = new StringTokenizer(nextLine, " \n\t\r");
						token = st.nextToken();
						width = Integer.parseInt(token);
						token = st.nextToken();
						height = Integer.parseInt(token);
						lineNo++;
					}
				}
			}

			image = new double[height][width];

			nextLine = br.readLine();
			while (br.ready()) {
				image[i][j] = br.read();
				j++;
				if (j == width) {
					i++;
					j = 0;	
				}
			}
			br.close();
		} catch(FileNotFoundException e) {
			System.err.println("File Not Found");
		} catch(IOException e) {
			System.err.println("IO Exception");
		}
		return image;
	}
			
	public static void writeImage(double[][] image, String outFile) {
		try {
			FileWriter fw = new FileWriter(outFile);
			BufferedWriter bw = new BufferedWriter(fw);

			int height = image.length;
			int width = image[0].length;

			bw.write("P5\n");
			bw.write(width + " " + height + "\n");
			bw.write(255 + "\n");
			for (int i = 0; i < height; i++)
				for (int j = 0; j < width; j++) 
					bw.write((char) image[i][j]);
			bw.close();
		} catch(IOException e) {
			System.err.println("IO Exception");
		} 		
	}

}

class Point {
	int i, j;

	public Point(int i, int j) {
		this.i = i;
		this.j = j;
	}
}

