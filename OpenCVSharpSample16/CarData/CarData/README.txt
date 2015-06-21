
---------------------------------------------------
UIUC Image Database for Car Detection : README File
---------------------------------------------------


Besides this README file, the CarData/ directory (to which the
download .tar file unpacks) contains the following:


1. Training images
------------------

These are located in CarData/TrainImages/.
The positive (car) images are named pos-n.pgm (0 <= n <= 549). 
The negative (non-car) images are named similarly as neg-n.pgm 
(0 <= n <= 499). 

The training images are all 100x40 pixels in size.


2. Test images - Single-scale
-----------------------------

These are located in CarData/TestImages/ and are named test-n.pgm
(0 <= n <= 169). The images are of different sizes themselves but
contain cars of approximately the same scale as in the training
images. These test images contain 200 cars in all.


3. Test images - Multi-scale
----------------------------

These are located in CarData/TestImages_Scale/ and are named test-n.pgm
(0 <= n <= 107). The images are of different sizes and contain cars of 
various scales. These test images contain 139 cars in all.


4. Evaluation files - Single-scale
----------------------------------

There are four files:

(a) trueLocations.txt

	This contains the true locations of the objects (cars) in the first 
	set of test images. The locations have been determined manually and are
	represented by the coordinates of the top-left corner of the best
	100x40 window containing each car. The nth row in the file
	corresponds to the nth test image; for example, if the nth test
	image contains two cars located at (i1,j1) and (i2,j2)
	respectively (where i denotes the height axis and j the width axis
	in usual image matrix notation), then the nth row looks like:

	n: (i1,j1) (i2,j2)

(b) Point.class

	This file is simply required by the evaluator program; it can be
	ignored.

(c) Evaluator.java

	This contains the source code for the evaluator program; it is
	provided to the user in case some modification is desired, but if
	using the evaluation scheme as provided, this file can be ignored.

(d) Evaluator.class

	This can be used to evaluate the performance of a detection
	algorithm on the test set. If the object locations determined by
	the algorithm are output to a file "foundLocations.txt" (which
	must be in the same format as the "trueLocations.txt" file
	described in part (a) above), the evaluation can be performed by
	running the command

	~CarData> java Evaluator trueLocations.txt foundLocations.txt

	This determines the number of correct and false detections, and
	outputs the results in the form of recall, precision and F-measure. 
	It also produces a file called "break-up.txt" which contains a 
	break-up of the correct and false detections for each test image. 
	(A location output by the algorithm is counted as a correct detection
	if it lies within an ellipse with center at the true location and 
	axes 25% of the object dimensions in each direction. In addition, 
	only one detection per object is allowed - if two or more detected 
	windows satisfy the above criteria for the same object, only one is 
	counted as correct; the others are counted as false detections.)

	The evaluator program can also be used to display the locations
	output by the algorithm as windows overlaid on the test images;
	correct detections are shown as complete windows, false detections
	as broken windows. This can be done by creating a directory, say
	"ResultImages", under CarData/ and then running:

	~CarData> java Evaluator trueLocations.txt foundLocations.txt
			  display TestImages ResultImages

	The results are produced as images named "result-n.pgm" that can
	be found in CarData/ResultImages/.

	This feature can also be used to display the test images with the
	true object locations; create a directory "LabeledImages" under
	CarData/ and run:

	~CarData> java Evaluator trueLocations.txt trueLocations.txt
			  display TestImages LabeledImages

	The labeled images are produced as CarData/LabeledImages/result-n.pgm.
	

5. Evaluation files - Multi-scale
---------------------------------

There are four files:

(a) trueLocations_Scale.txt

	This contains the true locations and scales of the objects (cars) in 
	the second set of test images. The locations and scales have been 
	determined manually and are represented by the coordinates of the 
	top-left corner and the width of the best window containing each car. 
  The nth row in the file corresponds to the nth test image; for example, 
  if the nth test image contains two cars with location-scale pairs
	(i1,j1,w1) and (i2,j2,w2) respectively (where i denotes the height axis 
	and j the width axis in usual image matrix notation), then the nth row 
	looks like:

	n: (i1,j1,w1) (i2,j2,w2)

(b) Point_Scale.class

	This file is simply required by the evaluator program; it can be
	ignored.

(c) Evaluator_Scale.java

	This contains the source code for the evaluator program; it is
	provided to the user in case some modification is desired, but if
	using the evaluation scheme as provided, this file can be ignored.

(d) Evaluator_Scale.class

	This can be used to evaluate the performance of a detection
	algorithm on the test set. If the object locations and scales 
	determined by the algorithm are output to a file 
	"foundLocations_Scale.txt" (which must be in the same format as 
	the "trueLocations_Scale.txt" file described in part (a) above), the 
	evaluation can be performed by running the command

	~CarData> java Evaluator_Scale trueLocations_Scale.txt 
	               foundLocations_Scale.txt

	This determines the number of correct and false detections, and
	outputs the results in the form of recall, precision and F-measure. 
	It also produces a file called "break-up_Scale.txt" which contains a 
	break-up of the correct and false detections for each test image.
	(A location-scale pair output by the algorithm is counted as a correct 
	detection if it lies within an ellipsoid with center at the true 
	location-scale and axes 25% of the true object dimensions in each 
	direction. In addition, only one detection per object is allowed - if 
	two or more detected windows satisfy the above criteria for the same 
	object, only one is counted as correct; the others are counted as 
	false detections.)

	The evaluator program can also be used to display the locations
	output by the algorithm as windows overlaid on the test images;
	correct detections are shown as complete windows, false detections
	as broken windows. This can be done as described for the single-scale
	case in 4(d) above.


-------------------------------------------------------------------------------

