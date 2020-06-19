# Импорт библиотек
from sklearn import datasets
from sklearn.manifold import TSNE
import matplotlib.pyplot as plt

# Загрузка датасета
iris_df = datasets.load_iris()
# chtoto = {data:array([[5.1, 3.5, 1.4, 0.2],
#        [4.9, 3. , 1.4, 0.2],
#        [4.7, 3.2, 1.3, 0.2],
#        [4.6, 3.1, 1.5, 0.2],
#        [5. , 3.6, 1.4, 0.2],
#        [5.4, 3.9, 1.7, 0.4],
#        [4.6, 3.4, 1.4, 0.3],
#        [5. , 3.4, 1.5, 0.2],
#        [4.4, 2.9, 1.4, 0.2],
#        [4.9, 3.1, 1.5, 0.1],
#        [5.4, 3.7, 1.5, 0.2],
#        [4.8, 3.4, 1.6, 0.2],
#        [4.8, 3. , 1.4, 0.1],

# Определяем модель и скорость обучения
model = TSNE(learning_rate=100)
iris_df = {'data': array([1, 2, 4, 100, 101, 102])}
# Обучаем модель
transformed = model.fit_transform(iris_df.data)

# Представляем результат в двумерных координатах
x_axis = transformed[:, 0]
y_axis = transformed[:, 1]

plt.scatter(x_axis, y_axis, c=iris_df.target)
plt.show()
print('zalupa')