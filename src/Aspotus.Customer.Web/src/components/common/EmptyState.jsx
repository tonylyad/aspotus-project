import { FiSearch } from "react-icons/fi"
import { useNavigate } from "react-router-dom";

export default function EmptyState({ title = "Ничего не найдено", text = "Попробуйте изменить параметры поиска." }) {
    const navigate = useNavigate();
    return (
        <div className="catalog-empty">
            <div className="catalog-empty__icon"><FiSearch /></div>
            <h3>{title}</h3>
            <p>{text}</p>
            <h3 style={{ margin: 10 }}>Или оставьте заявку, мы постараемся вам помочь</h3>
            <button onClick={() => navigate("/request")}>Оставить заявку</button>
        </div>
    )
}
