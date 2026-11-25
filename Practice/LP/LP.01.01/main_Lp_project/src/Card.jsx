import { useState } from 'react'

function Card({title, imageUrl}) 
{
    const [count, setCount] = useState(0)

    return (
        <>
        <div className='card'>
            <img src={imageUrl} alt={title} className='card-image' />    
            <h1>{title}</h1>
                <div className="card-buttons">
                {
                    count > 0 ?<>
                    
                        <button className="card-button" onClick={() => setCount((count) => count +1)}>+</button>
                        <p>В корзине: {count}</p>
                        {
                        count <0 ? setCount(0) : null 
                        }
                        <button className="card-button" onClick={() => setCount((count) => count -1)}>-</button>
                    </> : <button className="card-button-main   " onClick={() => setCount(1)}>Добавить в корзину</button>
                }
                </div>
        </div>
        </> 
    )   
}
export default Card