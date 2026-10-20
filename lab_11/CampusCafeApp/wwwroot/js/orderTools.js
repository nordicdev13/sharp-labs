export function confirmRemoval(name) {
    return window.confirm(`Видалити «${name}» з кошика?`);
}

export async function copySummary(text, receiver) {
    if (!window.isSecureContext || !navigator.clipboard) return false;
    try {
        await navigator.clipboard.writeText(text);
    } catch {
        return false;
    }
    // Приклад зворотного виклику JavaScript → екземпляр компонента C#.
    await receiver.invokeMethodAsync("OnCopied");
    return true;
}
