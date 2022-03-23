# Metody sztucznej inteligencji (Artificial intelligence fundamentals)
Pełna dokumentacja w folderze Docs.
## STRIPS - świat klocków
#### Założenia świata klocków:
* powierzchnia/płaszczyzna/podłoże, na którym umieszczamy klocki jest gładka i nieograniczona
* wszystkie klocki mają takie same rozmiary
* klocki mogą być umieszczone jeden na drugim
* klocki mogą tworzyć stosy
* położenie horyzontalne klocków jest nieistotne, liczy się ich wertykalne położenie względem siebie
* manipulujemy klockami tylko za pomocą ramienia robota
* w danej chwili w ramieniu robota może znajdować się tylko jeden klocek

#### Zbiór operatorów:
1. STACK(x,y): umieszczenie klocka x na klocku y
    * w ramieniu robota musi znajdować się klocek x, a na klocku y nie może znajdować się żaden klocek
2. UNSTACK(x,y): zdjęcie klocka x z klocka y
    * ramię robota musi być puste/wolne a na klocku x nie może znajdować się inny klocek
3. PICKUP(x): podniesienie klocka x z podłoża
    * ramię robota musi być puste/wolne a na klocku x nie może znajdować się inny klocek
4. PUTDOWN(x): umieszczenie klocka x na podłożu
    * w ramieniu robota musi znajdować się klocek x

#### Zbiór predykatów
1. ON(x,y): spełniony, gdy klocek x znajduje się na klocku y
2. ONTABLE(x): spełniony, gdy klocek x znajduje się bezpośrednio na podłożu
3. CLEAR(x): spełniony, gdy powierzchnia klocka x jest pusta tzn. nie znajduje się na nim żaden inny klocek
4. HOLDING(x): spełniony, gdy w ramieniu robota znajduje się klocek x
5. ARMEMPTY: spełniony, gdy ramię robota jest puste/wolne

Definicja formalna operatorów
1. STACK(x,y):
    * PRECONDITION: CLEAR(y) && HOLDING(x)
    * DELETE: CLEAR(y) && HOLDING(x)
    * ADD: ARMEMPTY && ON(x,y)
2. UNSTACK(x,y):
    * PRECONDITION: ON(x,y) && CLEAR(x) && ARMEMPTY
    * DELETE: ON(x,y) && ARMEMPTY
    * ADD: HOLDING(x) && CLEAR(y)
3. PICKUP(x):
    * PRECONDITION: CLEAR(x) && ONTABLE(x) && ARMEMPTY
    * DELETE: ONTABLE(x) && ARMEMPTY
    * ADD: HOLDING(x)
4. PUTDOWN(x):
    * PRECONDITION: HOLDING(x)
    * DELETE: HOLDING(x)
    * ADD: ONTABLE(x) && ARMEMPTY

## Cel i zadanie programu
Stworzenie generycznego algorytmu planowania zadań rozwiązujący problem znalezienia instrukcji w świecie klocków potrzebnych do otrzymania konkretnego stanu.

## Użycie
Aplikacja konsolowa przyjmuje jeden argument, ścieżkę do pliku konfiguracyjnego. 
W przypadku braku argumentu używa pliku conf.xml.
Domyslne ustawienia
```
  <MaxIterations>100</MaxIterations>
  <PathToProblem>..\..\..\problem.txt</PathToProblem>
  <PopulationSize>1000</PopulationSize>
  <OutputPath>.\output.txt</OutputPath>
```
