import { Button } from "react-bootstrap"
import { useNavigate } from "react-router-dom"
export default function BackButton() {


    const navigate = useNavigate();

    return(
        <Button variant="outline-secondary" onClick={() => navigate(-1)}>
            ← Назад
        </Button>
    )
}