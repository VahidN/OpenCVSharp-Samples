"E:\path\opencv\build\x86\vc12\bin\opencv_createsamples.exe" -info carImages.txt -num 550 -w 48 -h 24 -vec cars.vec
rem "E:\path\opencv\build\x86\vc12\bin\opencv_createsamples.exe" -vec cars.vec -w 48 -h 24
"E:\path\opencv\build\x86\vc12\bin\opencv_traincascade.exe" -data data -vec cars.vec -bg negativeImages.txt -numPos 500 -numNeg 500 -numStages 2 -w 48 -h 24 -featureType LBP
pause