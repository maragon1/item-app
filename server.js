//fetch the list of items from localhost:5000/items
const fetchItems = async () => {
  try {
    const response = await fetch('http://localhost:5000/items');
    const data = await response.json();
    return data;
  } catch (error) {
    console.error('Error fetching items:', error);
    return [];
  }
};

//post a new item to localhost:5000/items
const postItem = async (item) => {
  try {
    const response = await fetch('http://localhost:5000/items', {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json'
      },
      body: JSON.stringify(item)
    });
    const data = await response.json();
    return data;
  } catch (error) {
    console.error('Error posting item:', error);
    return null;
  }
};
