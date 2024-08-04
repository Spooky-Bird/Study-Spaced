@echo off

set/p input="Enter chapter: "
if %input%==1a set "chapter=1A Circular functions"
if %input%==1b set "chapter=1B The sine and cosine rules"
if %input%==1c set "chapter=1C Sequences and series"
if %input%==1d set "chapter=1D The modulus function"
if %input%==1e set "chapter=1E Circles"
if %input%==1f set "chapter=1F Ellipses and hyperbolas"
if %input%==1g set "chapter=1G Parametric equations"
if %input%==1h set "chapter=1H Algorithms and pseudocode"
if %input%==2a set "chapter=2A Revision of proof techniques"
if %input%==2b set "chapter=2B Quantifiers and counterexamples"
if %input%==2c set "chapter=2C Proving inequalities"
if %input%==2d set "chapter=2D Telescoping series"
if %input%==2e set "chapter=2E Mathematical induction"
if %input%==3a set "chapter=3A The reciprocal circular functions"
if %input%==3b set "chapter=3B Compound and double angle formulas"
if %input%==3c set "chapter=3C The inverse circular functions"
if %input%==3d set "chapter=3D Solution of equations"
if %input%==3e set "chapter=3E Sums and products of sines and cosines"
if %input%==4a set "chapter=4A Introduction to vectors"
if %input%==4b set "chapter=4B Resolution of a vector into rectangular components"
if %input%==4c set "chapter=4C Scalar product of vectors"
if %input%==4d set "chapter=4D Vector projections"
if %input%==4e set "chapter=4E Collinearity"
if %input%==4f set "chapter=4F Geometric proofs"
if %input%==5a set "chapter=5A Vector equations of lines"
if %input%==5b set "chapter=5B Intersection of lines and skew lines"
if %input%==5c set "chapter=5C Vector product"
if %input%==5d set "chapter=5D Vector equations of planes"
if %input%==5e set "chapter=5E Distances, angles and intersections"
if %input%==6a set "chapter=6A Starting to build the complex numbers"
if %input%==6b set "chapter=6B Modulus, conjugate and division"
if %input%==6c set "chapter=6C Polar form of a complex number"
if %input%==6d set "chapter=6D Basic operations on complex numbers in polar form"
if %input%==6e set "chapter=6E Solving quadratic equations over the complex numbers"
if %input%==6f set "chapter=6F Solving polynomial equations over the complex numbers"
if %input%==6g set "chapter=6G Using De Moivre’s theorem to solve equations"
if %input%==6h set "chapter=6H Sketching subsets of the complex plane"
if %input%==7a set "chapter=7A Technology-free questions"
if %input%==7b set "chapter=7B Multiple-choice questions"
if %input%==7c set "chapter=7C Extended-response questions"
if %input%==7d set "chapter=7D Algorithms and pseudocode"
if %input%==8a set "chapter=8A Differentiation"
if %input%==8b set "chapter=8B Derivatives of x = f(y)"
if %input%==8c set "chapter=8C Derivatives of inverse circular functions"
if %input%==8d set "chapter=8D Second derivatives"
if %input%==8e set "chapter=8E Points of inflection"
if %input%==8f set "chapter=8F Related rates"
if %input%==8g set "chapter=8G Rational functions"
if %input%==8h set "chapter=8H A summary of differentiation"
if %input%==8i set "chapter=8I Implicit differentiation"
if %input%==9a set "chapter=9A Antidifferentiation"
if %input%==9b set "chapter=9B Antiderivatives involving inverse circular functions"
if %input%==9c set "chapter=9C Integration by substitution"
if %input%==9d set "chapter=9D Definite integrals by substitution"
if %input%==9e set "chapter=9E Use of trigonometric identities for integration"
if %input%==9f set "chapter=9F Further substitution"
if %input%==9g set "chapter=9G Partial fractions"
if %input%==9h set "chapter=9H Integration by parts"
if %input%==9i set "chapter=9I Further techniques and miscellaneous exercises"
if %input%==10a set "chapter=10A The fundamental theorem of calculus"
if %input%==10b set "chapter=10B Area of a region between two curves"
if %input%==10c set "chapter=10C Integration using a CAS calculator"
if %input%==10d set "chapter=10D Volumes of solids of revolution"
if %input%==10e set "chapter=10E Lengths of curves in the plane"
if %input%==10f set "chapter=10F Areas of surfaces of revolution"
if %input%==11a set "chapter=11A An introduction to differential equations"
if %input%==11b set "chapter=11B Differential equations involving a function of the independent variable"
if %input%==11c set "chapter=11C Differential equations involving a function of the dependent variable"
if %input%==11d set "chapter=11D Applications of differential equations"
if %input%==11e set "chapter=11E The logistic differential equation"
if %input%==11f set "chapter=11F Separation of variables"
if %input%==11g set "chapter=11G Differential equations with related rates"
if %input%==11h set "chapter=11H Using a definite integral to solve a differential equation"
if %input%==11i set "chapter=11I Using Euler%27s method to solve a differential equation"
if %input%==11j set "chapter=11J Slope field for a differential equation"
if %input%==12a set "chapter=12A Position, velocity and acceleration"
if %input%==12b set "chapter=12B Constant acceleration"
if %input%==12c set "chapter=12C Velocity–time graphs"
if %input%==12d set "chapter=12D Differential equations of the form v = f(x) and a = f(v)"
if %input%==12e set "chapter=12E Other expressions for acceleration"
if %input%==13a set "chapter=13A Vector functions"
if %input%==13b set "chapter=13B Position vectors as a function of time"
if %input%==13c set "chapter=13C Vector calculus"
if %input%==13d set "chapter=13D Velocity and acceleration for motion along a curve"
if %input%==14a set "chapter=14A Technology-free questions"
if %input%==14b set "chapter=14B Multiple-choice questions"
if %input%==14c set "chapter=14C Extended-response questions"
if %input%==14d set "chapter=14D Algorithms and pseudocode"
if %input%==15a set "chapter=15A Linear functions of a random variable"
if %input%==15b set "chapter=15B Linear combinations of random variables"
if %input%==15c set "chapter=15C Linear combinations of normal random variables"
if %input%==15d set "chapter=15D The sample mean of a normal random variable"
if %input%==15e set "chapter=15E Investigating the distribution of the sample mean using simulatation"
if %input%==15f set "chapter=15F The distribution of the sample mean"
if %input%==16a set "chapter=16A Confidence intervals for the population mean"
if %input%==16b set "chapter=16B Hypothesis testing for the mean"
if %input%==16c set "chapter=16C One-tail and two-tail tests"
if %input%==16d set "chapter=16D Two-tail tests revisited"
if %input%==16e set "chapter=16E Errors in hypothesis testing"
if %input%==17a set "chapter=17A Technology-free questions"
if %input%==17b set "chapter=17B Multiple-choice questions"
if %input%==17c set "chapter=17C Extended-response questions"
if %input%==17d set "chapter=17D Algorithms and pseudocode"
if %input%==18a set "chapter=18A Technology-free questions"
if %input%==18b set "chapter=18B Multiple-choice questions"
if %input%==18c set "chapter=18C Extended-response questions"


start https://drive.google.com/drive/folders/14NezzRRCt9ynf7G5geZ--2Szd-1edF4O
start chrome "file:///C:/Users/%USERNAME%/OneDrive%%20-%%20Northcote%%20High%%20School/Subjects/Specialist/Answers.pdf"
start chrome "file:///C:/Users/%USERNAME%/OneDrive%%20-%%20Northcote%%20High%%20School/Subjects/Specialist/Textbook.pdf#%chapter%"