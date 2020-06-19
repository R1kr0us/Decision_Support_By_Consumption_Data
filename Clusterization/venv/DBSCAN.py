import numpy as np

from sklearn.cluster import DBSCAN
from sklearn import metrics
from sklearn.datasets import make_blobs
from sklearn.preprocessing import StandardScaler
import pandas as pd
import matplotlib.pyplot as plt


# #############################################################################
# Generate sample data
# centers = [[1, 1], [-1, -1], [1, -1]]
# X, labels_true = make_blobs(n_samples=750, centers=centers, cluster_std=0.4,
#                              random_state=0)
#
# X = StandardScaler().fit_transform(X)
# X = StandardScaler().fit_transform([[123.23, 150.23, 24.56],[103.23, 148.23, 22.56],[183.23, 130.23, 21.56],[210.23, 140.23, 23.56]])
# X = StandardScaler().fit_transform([[123.23, 150.23],[103.23, 148.23],[183.23, 130.23],[210.23, 140.23]])
# #############################################################################
# Compute DBSCAN

data = pd.read_excel("D:/Универ/new_try.xlsx", sheet_name='сентябрь2018')
X = data.to_numpy()
inputDate = []
for i in range(len(X)):
    if int(X[i,0]) == 0:
        inputDate.append(i)
X = np.delete(X,inputDate,axis=0)
for val in X:
    if val[0] == 0.0:
        X = np.delete(X,i,0)
# db = DBSCAN(eps=12, min_samples=7).fit(X)
#октябрь2018 полное db = DBSCAN(eps=5, min_samples= 8).fit(X)
#выходные дни
#db = DBSCAN(eps=6, min_samples= 7).fit(X)
db = DBSCAN(eps=6, min_samples= 7).fit(X)
# раб время
#db = DBSCAN(eps=2, min_samples= 8).fit(X)
#нераб время
#db = DBSCAN(eps=4, min_samples= 8).fit(X)
core_samples_mask = np.zeros_like(db.labels_, dtype=bool)
core_samples_mask[db.core_sample_indices_] = True
labels = db.labels_

# Number of clusters in labels, ignoring noise if present.
n_clusters_ = len(set(labels)) - (1 if -1 in labels else 0)
n_noise_ = list(labels).count(-1)

unique_labels = set(labels)
colors = [plt.cm.Spectral(each)
          for each in np.linspace(0, 1, len(unique_labels))]
numberOfClass = 0 #for label of legend
finalValueOfCons = 0
finalCostOfCons = 0
for k, col in zip(unique_labels, colors):
    numberOfClass += 1
    legendLabel = str(numberOfClass) + ' тип потерь'
    if k == -1:
        # Black used for noise.
        legendLabel = 'Шум'
        col = [0, 0, 0, 1]

    class_member_mask = (labels == k)

    xy = X[class_member_mask & core_samples_mask]
    consumptionSumm = np.sum(xy[:,0])
    finalValueOfCons += consumptionSumm
    finalCostOfCons += consumptionSumm*8
    print(consumptionSumm)
    print(consumptionSumm*8)
    plt.plot(xy[:, 0], xy[:, 1], 'o', markerfacecolor=tuple(col),
             markeredgecolor='k', markersize=14)

    xy = X[class_member_mask & ~core_samples_mask]
    plt.plot(xy[:, 0], xy[:, 1], 'o', markerfacecolor=tuple(col),
             markeredgecolor='k', markersize=6, label= legendLabel)
    plt.legend()
plt.title('Предполагаемое количество кластеров: %d' % n_clusters_)
print(finalValueOfCons)
print(finalCostOfCons)
plt.show()

print('')