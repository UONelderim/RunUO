# uzyte do wygenerowania zawartosci Nieumarlych.txt
# zawiera random, wiec wynik bedzie inny za kazdym razem

import random
import array

alphabet = 'aąbcćdeęfghijklłmnńoóprsśtuwyzźżqvx'
use = 'aeghmorsuyz'

polishPairs = []
for i in alphabet:
    for j in alphabet:
        polishPairs.append(i+j)

undeadPairs = []
for i in use:
    for j in use:
        for k in use:
            undeadPairs.append(i+j+k)

print('polish pairs length: ' + str(len(polishPairs)))
print('undead pairs length: ' + str(len(undeadPairs)))

if True:
    undeadPairs = undeadPairs[0:len(polishPairs)]
    random.shuffle(undeadPairs)

    for i in range(len(polishPairs)):
        print(polishPairs[i] + '=' + undeadPairs[i])
