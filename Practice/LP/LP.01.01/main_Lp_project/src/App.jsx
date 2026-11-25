import { useState } from 'react'
import './App.css'
import Card from './Card.jsx'

function App() {



  const [title, setTitle] = useState("")
  const [imageUrl, setImageUrl] = useState("")
  const [price, setPrice] = useState("")

  const [products, setProducts] = useState([])
  const [isAdd, setIsAdd] = useState(false)


  function hendleChange(event) {
    setTitle(event.target.value)

  }
  function hendleChangeImage(event) {
    setImageUrl(event.target.value)
  }
  function hendleChangePrice(event) {
    setPrice(event.target.value)
  }
  function hendleSubmit(event) {
    event.preventDefault()
    setProducts([...products, { title, imageUrl, price }])
    setTitle("")  
    setImageUrl("")
    setPrice("")
    setIsAdd(false) 
  }

  return (
    <>
    <h1>Товыры, которые понравятся всем! </h1>
    <div>
      <button onClick={() => setIsAdd(!isAdd)}>{isAdd ? "Отмена":"Добавить продукт"}</button>
    </div>
    <div>
      { isAdd ? (
        <form onSubmit={hendleSubmit}>
          <label>
            Введите название продукта:
            <input type="text" value={title} onChange={hendleChange} />
          </label>
          <label>
            Введите цену продукта:
            <input type="number" value={price} onChange={hendleChangePrice} />
          </label>
          <label>
            Введите ссылку на изображение продукта:
            <input type="text" value={imageUrl} onChange={hendleChangeImage} />
          </label>
          <button type="submit">Добавить продукт</button>
        </form>
      ) : null }
    </div>
    <div className='card-container'>
       <Card key={Date.now()} price={1000} title={"Заглушка"} imageUrl={"https://n1s1.hsmedia.ru/81/51/3e/81513ed38a5e9408891b88c923ceab60/607x597_0xac120003_11829355741615816995.jpg"} />
        <Card key={Date.now() + 1} price={1000} title={"Заглушка"} imageUrl={"https://n1s1.hsmedia.ru/81/51/3e/81513ed38a5e9408891b88c923ceab60/607x597_0xac120003_11829355741615816995.jpg"} />
        <Card key={Date.now() + 2} price={1000} title={"Заглушка"} imageUrl={"https://n1s1.hsmedia.ru/81/51/3e/81513ed38a5e9408891b88c923ceab60/607x597_0xac120003_11829355741615816995.jpg"} />
        <Card key={Date.now() + 3} price={1000} title={"Заглушка"} imageUrl={"https://n1s1.hsmedia.ru/81/51/3e/81513ed38a5e9408891b88c923ceab60/607x597_0xac120003_11829355741615816995.jpg"} />
        <Card key={Date.now() + 4} price={1000} title={"Заглушка"} imageUrl={"https://n1s1.hsmedia.ru/81/51/3e/81513ed38a5e9408891b88c923ceab60/607x597_0xac120003_11829355741615816995.jpg"} />
      {products.map((product, index) => (<Card key={index} title={product.title}  price={product.price} imageUrl={product.imageUrl} />))}
    </div>
    </>
  )
  
}


export default App