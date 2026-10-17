import java.io.File
import java.io.FileNotFoundException
import java.math.BigDecimal

data class OrderLine(val productCode: String, val quantity: Int, val unitPrice: BigDecimal) {
    val lineTotal: BigDecimal get() = unitPrice * BigDecimal(quantity)
}

class Order(val orderId: String, val lines: List<OrderLine>) {
    val total: BigDecimal get() = lines.fold(BigDecimal.ZERO) { sum, line -> sum + line.lineTotal }
}

interface OrderLineValidator {
    fun validate(line: OrderLine): String?
}

class OrderProcessor {
    var onRejected: (Int, String) -> Unit = { _, _ -> }
    fun reject(lineNumber: Int, reason: String) = onRejected(lineNumber, reason)
}

fun highValue(orders: List<Order>, threshold: BigDecimal): List<Order> =
    orders.filter { it.total > threshold }

// extension function
fun List<Order>.highValueOrders(threshold: BigDecimal): List<Order> = filter { it.total > threshold }

fun main(args: Array<String>) {
    val pen = OrderLine("PEN", 10, BigDecimal("1.50"))
    val book = OrderLine("BOOK", 2, BigDecimal("24.99"))
    val small = Order("1001", listOf(pen, book))
    val desk = Order("1002", listOf(OrderLine("DESK", 1, BigDecimal("520.00"))))
    println("[Simplicity] total: ${small.total}")
    println("[Expressivity] ${highValue(listOf(small, desk), BigDecimal("500.00")).map { it.orderId }}")

    val high = listOf(small, desk).highValueOrders(BigDecimal("500.00"))
    println("[Extensions] ${high.map { it.orderId }}")

    val byId = HashMap<String, Order>()
    val found: Order? = byId["x"]
    println("[Type checking] ${found?.orderId ?: "no such order"}")

    val quantity: Int? = try { "abc".toInt() } catch (e: NumberFormatException) { null }
    val text: String? = try { File("no-such-file.csv").bufferedReader().use { it.readText() } } catch (e: FileNotFoundException) { null }
    println("[Exceptions] $quantity $text")

    val p = OrderProcessor()
    p.onRejected = { line, reason -> println("[Syntax design] line $line: $reason") }
    p.reject(5, "quantity 'abc' is not a whole number")

    val backing = mutableListOf(pen)
    val view: List<OrderLine> = backing
    backing.add(book)
    println("[Aliasing] read-only view size after the mutable list changed: ${view.size}")
}
