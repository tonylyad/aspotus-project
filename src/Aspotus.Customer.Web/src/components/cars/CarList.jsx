import { Row, Col } from "react-bootstrap"
import CarCard from "./CarCard"

export default function CarList({ cars }) {

    return (

        <Row>

            {cars.map(car => (

                <Col
                    md={4}
                    className="mb-4"
                    key={car.id}
                >

                    <CarCard car={car} />

                </Col>

            ))}

        </Row>

    )

}