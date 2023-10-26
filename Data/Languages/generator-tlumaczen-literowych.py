# uzyte do wygenerowania zawartosci Nieumarlych.txt
# zawiera random, wiec wynik bedzie inny za kazdym razem

import random
import array

alphabet = 'aąbcćdeęfghijklłmnńoóprsśtuwyzźżqvx'
use = 'aghmorsu'

polishPairs = []
for i in alphabet:
    for j in alphabet:
        polishPairs.append(i+j)

undeadPairs = []
for i in use:
    for j in use:
        undeadPairs.append(i+j)

if True:
    random.shuffle(undeadPairs)

    for i in range(len(polishPairs)):
        print(polishPairs[i] + '=' + undeadPairs[i % len(undeadPairs)])
